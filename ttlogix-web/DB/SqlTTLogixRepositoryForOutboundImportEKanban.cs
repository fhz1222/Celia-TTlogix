using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TT.Common.Extensions;
using TT.Core.Entities;
using TT.Core.Enums;
using TT.Core.Interfaces;
using TT.Core.QueryFilters;
using TT.Core.QueryResults;

namespace TT.DB
{
    public class SqlTTLogixRepositoryForOutboundImportEKanban : ITTLogixRepositoryForOutboundImportEKanban
    {
        public SqlTTLogixRepositoryForOutboundImportEKanban(Context dbContext, IRawSqlExecutor rawSqlExecutor)
        {
            this.dbContext = dbContext;
            this.rawSqlExecutor = rawSqlExecutor;
        }

        public void PreloadDataToLocal(string jobNo, string orderNo, string factoryId, string supplierId, bool isCPart, string whsCode)
        {
            var _0 = dbContext.PickingLists
                .Where(x => x.JobNo == jobNo)
                .ToList();

            var _1 = dbContext.EOrders
                .Where(x => x.PurchaseOrderNo == orderNo)
                .ToList();

            var _2 = dbContext.EKanbanDetails
                .Where(x => x.OrderNo == orderNo)
                .ToList();

            var statuses = isCPart ? new StorageStatus[] { StorageStatus.Putaway, StorageStatus.Allocated } : new StorageStatus[] { StorageStatus.Putaway };
            var locationTypes = new LocationType[] { LocationType.Normal, LocationType.ExtSystem };
            var _3 = dbContext.StorageDetails
                .Join(
                    dbContext.Locations,
                    sd => new { Code = sd.LocationCode, sd.WHSCode },
                    l => new { l.Code, l.WHSCode },
                    (sd, l) => new { sd, l })
                .Where(x => x.sd.CustomerCode == factoryId)
                .Where(x => x.sd.Qty > 0)
                .Where(x => x.sd.OutJobNo == string.Empty)
                .Where(x => statuses.Contains(x.sd.Status))
                .Where(x => x.sd.WHSCode == whsCode)
                .Where(x => locationTypes.Contains(x.l.Type))
                .Where(x => x.sd.SupplierID == supplierId)
                .ToList();

            var _4 = dbContext.PartMasters
                .Where(x => x.CustomerCode == factoryId)
                .Where(x => _2.Select(x => x.SupplierID).Contains(x.SupplierID))
                .Where(x => _2.Select(x => x.ProductCode).Contains(x.ProductCode1))
                .ToList();

            var _5 = dbContext.Inventory
                .Where(x => x.WHSCode == whsCode)
                .Where(x => x.CustomerCode == factoryId)
                .Where(x => _2.Select(x => x.SupplierID).Contains(x.SupplierID))
                .Where(x => _2.Select(x => x.ProductCode).Contains(x.ProductCode1))
                .ToList();
        }

        #region Queryable collections
        public IQueryable<PickingList> PickingLists()
        {
            return dbContext.PickingLists.Local.AsQueryable();
        }
        public IQueryable<EKanbanDetail> EKanbanDetails()
        {
            return dbContext.EKanbanDetails.Local.AsQueryable();
        }
        public IQueryable<StorageDetail> StorageDetails()
        {
            return dbContext.StorageDetails.Local.AsQueryable();
        }
        public IQueryable<EOrder> EOrders()
        {
            return dbContext.EOrders.Local.AsQueryable();
        }

        #endregion

        #region queries

        public async Task<EOrder> GetFirstEOrder(string orderNo, string productCode)
        {
            return (from eorders in dbContext.EOrders.Local.AsQueryable()
                    where eorders.PurchaseOrderNo == orderNo
                    && eorders.PartNo == productCode
                    orderby eorders.CardSerial descending
                    select eorders).FirstOrDefault();
        }

        public async Task<IEnumerable<OutboundDetailPickingQueryResult>> GetOutboundDetailPickingResultList(string jobNo)
        {
            var groupedResults = ((from detail in dbContext.OutboundDetails.Local.AsQueryable()
                                         join pl in dbContext.PickingLists.Local.AsQueryable() on new { detail.JobNo, detail.ProductCode, detail.LineItem } equals new { pl.JobNo, pl.ProductCode, pl.LineItem }
                                         where detail.JobNo == jobNo
                                         orderby pl.SupplierID
                                         select new { detail, pl }).ToList())
                                .GroupBy(g => new { g.pl.JobNo, g.pl.LineItem, g.pl.ProductCode })
                                .Select(r => new OutboundDetailPickingQueryResult
                                {
                                    OutboundDetail = r.Select(v => v.detail).First(),
                                    TotalPickedQty = r.Where(l => !String.IsNullOrEmpty(l.pl.PickedBy)).Sum(l => l.pl.Qty),
                                    TotalPickedPkg = r.Count(l => l.pl.Qty > 0 && !String.IsNullOrEmpty(l.pl.PickedBy))
                                }).ToList();

            return groupedResults;
        }

        public async Task<IList<StorageDetailExtendedQueryResult>> GetStorageDetailListEuro(StorageDetailExtendedQueryFilter filter)
        {
            var query = GetStorageDetailWithLocationListQuery(filter);
            var result = query.Select(q => new StorageDetailExtendedQueryResult()
            {
                StorageDetail = q.StorageDetail,
                PID = q.StorageDetail.PID,
                InJobNo = q.StorageDetail.InJobNo,
                LineItem = q.StorageDetail.LineItem,
                SeqNo = q.StorageDetail.SeqNo,
                ProductCode = q.StorageDetail.ProductCode,
                CustomerCode = q.StorageDetail.CustomerCode,
                Qty = q.StorageDetail.Qty,
                AllocatedQty = q.StorageDetail.AllocatedQty,
                OutJobNo = q.StorageDetail.OutJobNo,
                LocationCode = q.StorageDetail.LocationCode,
                Status = q.StorageDetail.Status,
                SupplierID = q.StorageDetail.SupplierID,
                Ownership = q.StorageDetail.Ownership,
                LocationType = q.LocationType

            }).ToList();

            return result;
        }

        public async Task<IList<EKanbanDetail>> GetEKanbanDetailForPicking(EKanbanForPickingQueryFilter filter)
        {
            var query = dbContext.EKanbanDetails.Local.AsQueryable();
            if(filter.OrderNo != null)
            {
                query = query.Where(q => q.OrderNo == filter.OrderNo);
            }
            if(filter.ProductCode != null)
            {
                query = query.Where(q => q.ProductCode == filter.ProductCode);
            }
            if(filter.SupplierIdEmpty.HasValue)
            {
                query = query.Where(q => filter.SupplierIdEmpty.Value ? String.IsNullOrEmpty(q.SupplierID) : !String.IsNullOrEmpty(q.SupplierID));
            }
            return query.OrderBy(detail => detail.ProductCode).ThenBy(detail => detail.SupplierID).ToList();
        }

        private IQueryable<StorageDetailQueryable> GetStorageDetailWithLocationListQuery(StorageDetailExtendedQueryFilter filter)
        {
            var query = from storageDetail in dbContext.StorageDetails.Local.AsQueryable()
                        join location in dbContext.Locations.Local.AsQueryable() on new { Code = storageDetail.LocationCode, storageDetail.WHSCode }
                        equals new { location.Code, location.WHSCode }
                        select new StorageDetailQueryable()
                        {
                            StorageDetail = storageDetail,
                            LocationName = location.Name,
                            LocationType = location.Type,
                            LocationCode = location.Code,
                            IsPriority = location.IsPriority,
                            PutawayDateDate = storageDetail.PutawayDate != null ? storageDetail.PutawayDate.Value.Date : new Nullable<DateTime>()
                        };
            if (filter.LocationCodeNotEmpty)
            {
                query = query.Where(q => !String.IsNullOrEmpty(q.LocationCode));
            }
            if (filter.QtyGreaterThanZero)
            {
                query = query.Where(q => q.StorageDetail.Qty > 0 && q.StorageDetail.Qty - q.StorageDetail.AllocatedQty > 0);
            }
            if (filter.CustomerCode != null)
            {
                query = query.Where(q => q.StorageDetail.CustomerCode == filter.CustomerCode);
            }
            if (filter.SupplierId != null)
            {
                query = query.Where(q => q.StorageDetail.SupplierID == filter.SupplierId);
            }
            if (filter.ProductCode != null)
            {
                query = query.Where(q => q.StorageDetail.ProductCode == filter.ProductCode);
            }
            if (filter.QtyGreaterThan.HasValue)
            {
                query = query.Where(q => q.StorageDetail.Qty > filter.QtyGreaterThan);
            }
            if (filter.AllocatedQtyGreaterThanZero)
            {
                query = query.Where(q => q.StorageDetail.Qty - q.StorageDetail.AllocatedQty > 0);
            }
            if (filter.OutJobNo != null)
            {
                query = query.Where(q => q.StorageDetail.OutJobNo == filter.OutJobNo);
            }
            if (filter.WHSCode != null)
            {
                query = query.Where(q => q.StorageDetail.WHSCode == filter.WHSCode);
            }
            if (filter.Statuses?.Any() == true)
            {
                query = query.Where(q => filter.Statuses.Contains(q.StorageDetail.Status));
            }
            if (filter.Ownership != null)
            {
                query = query.Where(q => q.StorageDetail.Ownership == filter.Ownership);
            }
            if (filter.LocationTypes?.Any() == true)
            {
                query = query.Where(q => filter.LocationTypes.Contains(q.LocationType));
            }

            query = query.OrderByDescending(q => q.StorageDetail.Ownership)
                .ThenBy(q => q.StorageDetail.InboundDate)
                .ThenByDescending(q => q.IsPriority)
                .ThenBy(q => q.PutawayDateDate)
                .ThenBy(q => q.StorageDetail.InJobNo)
                .ThenBy(q => q.StorageDetail.Version)
                .ThenBy(q => q.LocationCode)
                .ThenBy(q => q.StorageDetail.PID);

            return query;
        }

        private class StorageDetailQueryable
        {
            public StorageDetail StorageDetail { get; set; }
            public Location Location { get; set; }
            public string LocationName { get; set; }
            public LocationType LocationType { get; set; }
            public string LocationCode { get; set; }
            public byte? IsPriority { get; set; }
            public DateTime? PutawayDateDate { get; set; }
        }

        #endregion

        #region get entity

        public async Task<Outbound> GetOutboundAsync(string jobNo)
        {
            return await dbContext.Outbounds.FindAsync(jobNo.Trim());
        }
        public async Task<OutboundDetail> GetOutboundDetailAsync(string jobNo, int lineItem)
        {
            return await dbContext.OutboundDetails.FindAsync(jobNo.Trim(), lineItem);
        }
        public async Task<PickingListEKanban> GetPickingListEKanbanAsync(string jobNo, int lineItem, int seqNo)
        {
            return await dbContext.PickingListEKanbans.FindAsync(jobNo.Trim(), lineItem, seqNo);
        }
        public async Task<PickingAllocatedPID> GetPickingAllocatedPIDAsync(string jobNo, int lineItem, int seqNo)
        {
            return await dbContext.PickingAllocatedPIDs.FindAsync(jobNo.Trim(), lineItem, seqNo);
        }

        public async Task<PartMaster> GetPartMasterAsync(string customerCode, string supplierId, string productCode)
        {
            return await dbContext.PartMasters.FindAsync(customerCode.Trim(), supplierId.Trim(), productCode.Trim());
        }
        public async Task<Inventory> GetInventoryAsync(string customerCode, string supplierID, string productCode, string wHSCode, Ownership ownership)
        {
            return await dbContext.Inventory.FindAsync(customerCode.Trim(), supplierID.Trim(), productCode.Trim(), wHSCode.Trim(), ownership);
        }
        public async Task<bool> EKanbanDetailExistsAsync(string orderNo, string productCode, string serialNo)
        {
            //can check only local bc we preloaded all data for this orderNo
            return dbContext.EKanbanDetails.Local
                .Where(x => x.OrderNo == orderNo.Trim())
                .Where(x => x.ProductCode == productCode.Trim())
                .Where(x => x.SerialNo == serialNo.Trim())
                .Any();
        }
        public async Task<EKanbanHeader> GetEKanbanHeaderAsync(string orderNo)
        {
            return await dbContext.EKanbanHeaders.FindAsync(orderNo.Trim());
        }
        public async Task<SupplierMaster> GetSupplierMasterAsync(string factoryId, string supplierId)
        {
            return await dbContext.SupplierMasters.FindAsync(factoryId.Trim(), supplierId.Trim());
        }

        #endregion

        #region add

        public async Task AddOutboundAsync(Outbound entity)
        {
            entity.CreatedDate = DateTime.Now;
            dbContext.Outbounds.Local.Add(entity);
        }
        public async Task AddOutboundDetailAsync(OutboundDetail entity)
        {
            entity.CreatedDate = DateTime.Now;
            dbContext.OutboundDetails.Local.Add(entity);
        }
        public async Task AddPickingListAsync(PickingList entity)
        {
            dbContext.PickingLists.Local.Add(entity);
        }

        public async Task AddEKanbanHeaderAsync(EKanbanHeader entity)
        {
            entity.CreatedDate = DateTime.Now;
            dbContext.EKanbanHeaders.Local.Add(entity);
        }

        public async Task AddEKanbanDetailAsync(EKanbanDetail entity)
        {
            dbContext.EKanbanDetails.Local.Add(entity);
        }

        public async Task AddEOrderAsync(EOrder entity)
        {
            dbContext.EOrders.Local.Add(entity);
        }
        public async Task AddPickingListEKanbanAsync(PickingListEKanban entity)
        {
            dbContext.PickingListEKanbans.Local.Add(entity);
        }
        public async Task AddPickingAllocatedPIDAsync(PickingAllocatedPID entity)
        {
            dbContext.PickingAllocatedPIDs.Local.Add(entity);
        }

        #endregion

        #region delete
        public async Task DeleteOutboundDetailAsync(OutboundDetail entity)
        {
            dbContext.OutboundDetails.Local.Remove(entity);
        }
        public async Task DeleteEKanbanDetailAsync(EKanbanDetail entity)
        {
            dbContext.EKanbanDetails.Local.Remove(entity);
        }
        public async Task DeleteEOrderAsync(EOrder entity)
        {
            dbContext.EOrders.Local.Remove(entity);
        }
        public async Task DeletePickingListAsync(PickingList entity)
        {
            dbContext.PickingLists.Local.Remove(entity);
        }
        public async Task DeletePickingAllocatedPIDAsync(PickingAllocatedPID entity)
        {
            dbContext.PickingAllocatedPIDs.Local.Remove(entity);
        }
        public async Task DeletePickingListEKanbanAsync(PickingListEKanban entity)
        {
            dbContext.PickingListEKanbans.Local.Remove(entity);
        }

        #endregion

        public void ChangeTrackingOff() => dbContext.ChangeTrackingOff();
        public void ChangeTrackingOn() => dbContext.ChangeTrackingOn();

        public async Task SaveChangesAsyncFinal()
        {
            await dbContext.SaveChangesAsync();
        }

        private readonly Context dbContext;
        private readonly IRawSqlExecutor rawSqlExecutor;
    }
}
