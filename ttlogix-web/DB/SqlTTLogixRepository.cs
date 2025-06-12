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
    public class SqlTTLogixRepository : ITTLogixRepository
    {
        public SqlTTLogixRepository(Context dbContext, IRawSqlExecutor rawSqlExecutor)
        {
            this.dbContext = dbContext;
            this.rawSqlExecutor = rawSqlExecutor;
        }

        #region Queryable collections

        public IQueryable<Outbound> Outbounds()
        {
            return dbContext.Outbounds.AsQueryable();
        }
        public IQueryable<Customer> Customers()
        {
            return dbContext.Customers.AsQueryable();
        }
        public IQueryable<OutboundDetail> OutboundDetails()
        {
            return dbContext.OutboundDetails.AsQueryable();
        }
        public IQueryable<SupplierMaster> SupplierMasters()
        {
            return dbContext.SupplierMasters.AsQueryable();
        }
        public IQueryable<PickingList> PickingLists()
        {
            return dbContext.PickingLists.AsQueryable();
        }
        public IQueryable<Inventory> Inventory()
        {
            return dbContext.Inventory.AsQueryable();
        }
        public IQueryable<EKanbanHeader> EKanbanHeaders()
        {
            return dbContext.EKanbanHeaders.AsQueryable();
        }
        public IQueryable<EKanbanDetail> EKanbanDetails()
        {
            return dbContext.EKanbanDetails.AsQueryable();
        }
        public IQueryable<Warehouse> Warehouses()
        {
            return dbContext.Warehouses.AsQueryable();
        }
        public IQueryable<PartMaster> PartMasters()
        {
            return dbContext.PartMasters.AsQueryable();
        }
        public IQueryable<UOM> UOMs()
        {
            return dbContext.UOMs.AsQueryable();
        }
        public IQueryable<UOMDecimal> UOMDecimals()
        {
            return dbContext.UOMDecimals.AsQueryable();
        }
        public IQueryable<ExternalPID> ExternalPIDs()
        {
            return dbContext.ExternalPIDs.AsQueryable();
        }
        public IQueryable<StorageDetail> StorageDetails()
        {
            return dbContext.StorageDetails.AsQueryable();
        }

        public IQueryable<StorageDetailGroup> StorageDetailGroups()
        {
            return dbContext.StorageDetailGroups.AsQueryable();
        }
        public IQueryable<Inbound> Inbounds()
        {
            return dbContext.Inbounds.AsQueryable();
        }
        public IQueryable<InboundDetail> InboundDetails()
        {
            return dbContext.InboundDetails.AsQueryable();
        }
        public IQueryable<PriceMaster> PriceMasters()
        {
            return dbContext.PriceMasters.AsQueryable();
        }

        public IQueryable<Location> Locations()
        {
            return dbContext.Locations.AsQueryable();
        }
        public IQueryable<EOrder> EOrders()
        {
            return dbContext.EOrders.AsQueryable();
        }
        public IQueryable<PickingListEKanban> PickingListEKanbans()
        {
            return dbContext.PickingListEKanbans.AsQueryable();
        }
        public IQueryable<PickingAllocatedPID> PickingAllocatedPIDs()
        {
            return dbContext.PickingAllocatedPIDs.AsQueryable();
        }
        public IQueryable<Loading> Loadings()
        {
            return dbContext.Loadings.AsQueryable();
        }
        public IQueryable<LoadingDetail> LoadingDetails()
        {
            return dbContext.LoadingDetails.AsQueryable();
        }
        public IQueryable<ReportPrintingLog> ReportPrintingLogs()
        {
            return dbContext.ReportPrintingLogs.AsQueryable();
        }
        public IQueryable<ASNHeader> ASNHeaders()
        {
            return dbContext.ASNHeaders.AsQueryable();
        }
        public IQueryable<ASNDetail> ASNDetails()
        {
            return dbContext.ASNDetails.AsQueryable();
        }
        public IQueryable<EPO> EPOs()
        {
            return dbContext.EPOs.AsQueryable();
        }
        public IQueryable<PackageType> PackageTypes()
        {
            return dbContext.PackageTypes.AsQueryable();
        }
        public IQueryable<QuarantineLog> QuarantineLogs()
        {
            return dbContext.QuarantineLogs.AsQueryable();
        }
        public IQueryable<SupplierItemMaster> SupplierItemMasters()
        {
            return dbContext.SupplierItemMasters.AsQueryable();
        }
        public IQueryable<FactoryMaster> FactoryMasters()
        {
            return dbContext.FactoryMasters.AsQueryable();
        }
        public IQueryable<Country> Countries()
        {
            return dbContext.Countries.AsQueryable();
        }
        public IQueryable<SunsetExpiredAlert> SunsetExpiredAlerts()
        {
            return dbContext.SunsetExpiredAlerts.AsQueryable();
        }
        public IQueryable<SupplierDetail> SupplierDetails()
        {
            return dbContext.SupplierDetails.AsQueryable();
        }
        public IQueryable<LabelPrinter> LabelPrinters()
        {
            return dbContext.LabelPrinters.AsQueryable();
        }
        public IQueryable<User> Users()
        {
            return dbContext.Users.AsQueryable();
        }
        public IQueryable<AccessGroup> AccessGroups()
        {
            return dbContext.AccessGroups.AsQueryable();
        }
        public IQueryable<SystemModule> SystemModules()
        {
            return dbContext.SystemModules.AsQueryable();
        }
        public IQueryable<AccessRight> AccessRights()
        {
            return dbContext.AccessRights.AsQueryable();
        }
        public IQueryable<StockTransfer> StockTransfers()
        {
            return dbContext.StockTransfers.AsQueryable();
        }
        public IQueryable<StockTransferDetail> StockTransferDetails()
        {
            return dbContext.StockTransferDetails.AsQueryable();
        }
        public IQueryable<EStockTransferHeader> EStockTransferHeaders()
        {
            return dbContext.EStockTransferHeaders.AsQueryable();
        }
        public IQueryable<Core.Entities.ILogLocationCategory> ILogLocationCategories()
        {
            return dbContext.ILogLocationCategories.AsQueryable();
        }

        #endregion

        #region queries
        private int GetILogLocationCategory(string categoryName)
        {
            return ILogLocationCategories().First(l => l.Name == categoryName).Id;
        }

        public IQueryable<T> GetOutboundList<T>(OutboundListQueryFilter filter) where T : OutboundListQueryResult, new()
        {

            var query = from outbound in dbContext.Outbounds
                        join customer in dbContext.Customers on outbound.CustomerCode equals customer.Code
                        where outbound.WHSCode == filter.UserWHSCode && customer.WHSCode == filter.UserWHSCode

                        select new
                        {
                            outbound,
                            SupplierNames = (from sm in dbContext.SupplierMasters
                                             join d in dbContext.OutboundDetails on sm.SupplierID equals d.SupplierID
                                             where d.JobNo == outbound.JobNo
                                             select sm.CompanyName
                                             ).ToList()
                        };

            if (filter.CreatedDate.HasValue)
            {
                var date = filter.CreatedDate.Value.Date;
                query = query.Where(o => o.outbound.CreatedDate.Date == date);
            }

            if (filter.DispatchedDate.HasValue)
            {
                var date = filter.DispatchedDate.Value.Date;
                query = query.Where(o => o.outbound.DispatchedDate.HasValue && o.outbound.DispatchedDate.Value.Date == date);
            }
            if (!String.IsNullOrWhiteSpace(filter.JobNo))
            {
                query = query.Where(o => o.outbound.JobNo.ToLower().Contains(filter.JobNo.ToLower()));
            }
            if (filter.CustomerCodes?.Any() == true)
            {
                var customerCodes = filter.CustomerCodes.Select(c => c.ToLower());
                query = query.Where(o => customerCodes.Contains(o.outbound.CustomerCode.ToLower()));
            }
            if (!String.IsNullOrWhiteSpace(filter.RefNo))
            {
                query = query.Where(o => o.outbound.RefNo.ToLower().Contains(filter.RefNo.ToLower()));
            }
            query = filter.Remark switch
            {
                _ when string.IsNullOrWhiteSpace(filter.Remark) => query,
                _ when filter.Remark.Contains('%') => query.Where(o => EF.Functions.Like(o.outbound.Remark, FormatForWildcardSearch(filter.Remark), EFCoreExtensions.ESCAPE_CHAR)),
                _ => query.Where(o => o.outbound.Remark == filter.Remark),
            };
            if (filter.TransType.HasValue)
            {
                query = query.Where(o => o.outbound.TransType == filter.TransType.Value);
            }
            if (filter.Statuses != null && filter.Statuses.Any())
            {
                query = query.Where(o => filter.Statuses.Contains(o.outbound.Status));
            }

            if (!String.IsNullOrWhiteSpace(filter.SupplierName))
            {
                query = query.Where(o => o.SupplierNames.Any(s => s.ToLower().Contains(filter.SupplierName.ToLower())));
            }

            if (filter.OrderBy != null)
            {
                switch (filter.OrderBy)
                {
                    case "jobNo":
                    case "createdDate":
                    case "dispatchedDate":
                    case "refNo":
                    case "remark":
                    case "transType":
                    case "status":
                        query = query.OrderBy("outbound." + filter.OrderBy + (filter.Desc ? " desc" : " asc"));
                        break;
                    case "customerName":
                        query = query.OrderBy("customer.Name" + (filter.Desc ? " desc" : " asc"));
                        break;
                    case "supplierName":
                        query = filter.Desc ? query.OrderByDescending(q => q.SupplierNames.First()) : query.OrderBy(q => q.SupplierNames.First());
                        break;
                    default:
                        break;
                }
            }
            else
                query = query.OrderBy("outbound.JobNo asc");

            return query.Select(r => new T()
            {
                JobNo = r.outbound.JobNo,
                CustomerCode = r.outbound.CustomerCode,
                RefNo = r.outbound.RefNo,
                WHSCode = r.outbound.WHSCode,
                CreatedDate = r.outbound.CreatedDate,
                DispatchedDate = r.outbound.DispatchedDate,
                TransType = Enum.Parse<OutboundType>(r.outbound.TransType.ToString()),
                Status = Enum.Parse<OutboundStatus>(r.outbound.Status.ToString()),
                SupplierNameList = r.SupplierNames.ToList(),
                Remark = r.outbound.Remark,
                XDock = r.outbound.XDock
            });
        }

        public IQueryable<T> GetStockTransferList<T>(StockTransferListQueryFilter filter) where T : StockTransferListQueryResult, new()
        {
            var query = from st in dbContext.StockTransfers
                        join customer in dbContext.Customers on st.CustomerCode equals customer.Code
                        where st.WHSCode == filter.UserWHSCode && customer.WHSCode == filter.UserWHSCode

                        select new
                        {
                            st,
                            SupplierNames = (from sm in dbContext.SupplierMasters
                                             join d in dbContext.StockTransferDetails on sm.SupplierID equals d.OriginalSupplierID
                                             where d.JobNo == st.JobNo
                                             select sm.CompanyName
                                             ).ToList()
                        };
            if (filter.CustomerCodes?.Any() == true)
            {
                var customerCodes = filter.CustomerCodes.Select(c => c.ToLower());
                query = query.Where(o => customerCodes.Contains(o.st.CustomerCode.ToLower()));
            }
            if (!String.IsNullOrWhiteSpace(filter.JobNo))
            {
                query = query.Where(o => o.st.JobNo.ToLower().Contains(filter.JobNo.ToLower()));
            }
            if (!String.IsNullOrWhiteSpace(filter.SupplierName))
            {
                query = query.Where(o => o.SupplierNames.Any(s => s.ToLower().Contains(filter.SupplierName.ToLower())));
            }
            if (!String.IsNullOrWhiteSpace(filter.RefNo))
            {
                query = query.Where(o => o.st.RefNo.ToLower().Contains(filter.RefNo.ToLower()));
            }
            if (filter.TransferType.HasValue)
            {
                query = query.Where(o => o.st.TransferType == filter.TransferType.Value);
            }
            if (filter.CreatedDate.HasValue)
            {
                var date = filter.CreatedDate.Value.Date;
                query = query.Where(o => o.st.CreatedDate.Date == date);
            }
            if (filter.Statuses != null && filter.Statuses.Any())
            {
                query = query.Where(o => filter.Statuses.Contains(o.st.Status));
            }
            if (!String.IsNullOrWhiteSpace(filter.Remark))
            {
                switch (filter.RemarkFilter ?? StringFilterMode.Contains)
                {
                    case StringFilterMode.Equals:
                        query = query.Where(o => o.st.Remark.ToLower().Equals(filter.Remark.ToLower()));
                        break;
                    case StringFilterMode.StartsWith:
                        query = query.Where(o => o.st.Remark.ToLower().StartsWith(filter.Remark.ToLower()));
                        break;
                    case StringFilterMode.EndsWith:
                        query = query.Where(o => o.st.Remark.ToLower().EndsWith(filter.Remark.ToLower()));
                        break;
                    case StringFilterMode.Contains:
                    default:
                        query = query.Where(o => o.st.Remark.ToLower().Contains(filter.Remark.ToLower()));
                        break;
                }
            }


            if (filter.OrderBy != null)
            {
                switch (filter.OrderBy)
                {
                    case "jobNo":
                    case "createdDate":
                    case "dispatchedDate":
                    case "refNo":
                    case "remark":
                    case "transferType":
                    case "status":
                        query = query.OrderBy("st." + filter.OrderBy + (filter.Desc ? " desc" : " asc"));
                        break;
                    case "customerName":
                        query = query.OrderBy("customer.Name" + (filter.Desc ? " desc" : " asc"));
                        break;
                    case "supplierName":
                        query = filter.Desc ? query.OrderByDescending(q => q.SupplierNames.First()) : query.OrderBy(q => q.SupplierNames.First());
                        break;
                    default:
                        break;
                }
            }
            else
                query = query.OrderBy("st.JobNo asc");

            return query.Select(r => new T()
            {
                JobNo = r.st.JobNo,
                CustomerCode = r.st.CustomerCode,
                RefNo = r.st.RefNo,
                WHSCode = r.st.WHSCode,
                CreatedDate = r.st.CreatedDate,
                TransferType = r.st.TransferType,
                Status = r.st.Status,
                SupplierNameList = r.SupplierNames.ToList(),
                Remark = r.st.Remark
            });

        }

        public IQueryable<T> GetEKanbanList<T>(EKanbanListQueryFilter filter) where T : EKanbanListQueryResult, new()
        {
            // for Europe
            const string SupplierInstructions = "Supplier";
            const string DeljitInstructions = "DELJIT";
            var query = from header in dbContext.EKanbanHeaders
                        
                        join customer in dbContext.Customers on header.FactoryID equals customer.Code

                        join df in dbContext.DelforHeaders on
                        header.RefNo equals df.EDIID into dfh
                        from delfor in dfh.DefaultIfEmpty()

                        join dj in dbContext.DeljitHeaders on
                        header.RefNo equals dj.EDIID into djh
                        from deljit in djh.DefaultIfEmpty()

                        join kanbanDetail in (from kd in dbContext.EKanbanDetails
                                              group new { kd.OrderNo, kd.SupplierID } by kd.OrderNo into g
                                              select new { OrderNo = g.Key, SupplierID = g.Min(g => g.SupplierID) }) on header.OrderNo equals kanbanDetail.OrderNo
                        
                        join sm in dbContext.SupplierMasters on
                        new { kanbanDetail.SupplierID, header.FactoryID } equals new { sm.SupplierID, sm.FactoryID } into s
                        from supplier in s.DefaultIfEmpty()
                        
                        where header.Status == (int)EKanbanStatus.New && customer.WHSCode == filter.UserWHSCode
                        
                        let delforEDIDate = delfor == null ? (DateTime?)null : delfor.EDIDate
                        let deljitEDIDate = deljit == null ? (DateTime?)null : deljit.EDIDate
                        let shouldDelforBePicked = header.Instructions == string.Empty
                        let shouldDeljitBePicked = header.Instructions == DeljitInstructions || header.Instructions == SupplierInstructions
                        select new
                        {
                            header.Status,
                            header.ETA,
                            header.OrderNo,
                            header.FactoryID,
                            BlanketOrderNo = header.Instructions == "EHP" ? header.RefNo : header.BlanketOrderNo,
                            header.CreatedDate,
                            SupplierId = kanbanDetail != null ? kanbanDetail.SupplierID : null,
                            SupplierName = supplier != null ? supplier.CompanyName : "EHP",
                            EDIDate = shouldDelforBePicked ? delforEDIDate : shouldDeljitBePicked ? deljitEDIDate : null
                        };

            if (!string.IsNullOrWhiteSpace(filter.OrderNo))
            {
                query = query.Where(h => h.OrderNo.ToLower().Contains(filter.OrderNo.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(filter.SupplierID))
            {
                query = query.Where(h => h.SupplierId.ToLower().Contains(filter.SupplierID.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(filter.SupplierName))
            {
                query = query.Where(h => h.SupplierName.ToLower().Contains(filter.SupplierName.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(filter.FactoryId))
            {
                query = query.Where(h => h.FactoryID.ToLower().Contains(filter.FactoryId.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(filter.BlanketOrderNo))
            {
                query = query.Where(h => h.BlanketOrderNo.ToLower().Contains(filter.BlanketOrderNo.ToLower()));
            }
            if (filter.ETAFrom.HasValue)
            {
                var date = filter.ETAFrom.Value.Date;
                query = query.Where(h => h.ETA.HasValue && h.ETA >= date);
            }
            if (filter.ETATo.HasValue)
            {
                var date = filter.ETATo.Value.Date.AddDays(1);
                query = query.Where(h => h.ETA.HasValue && h.ETA < date);
            }
            if (filter.EDIDateFrom.HasValue)
            {
                var ediDate = filter.EDIDateFrom.Value.Date;
                query = query.Where(h => h.EDIDate >= ediDate);
            }
            if (filter.EDIDateTo.HasValue)
            {
                var ediDate = filter.EDIDateTo.Value.Date.AddDays(1);
                query = query.Where(h => h.EDIDate < ediDate);
            }

            if (filter.OrderBy != null)
            {
                query = query.OrderBy(filter.OrderBy + (filter.Desc ? " desc" : " asc"));
            }
            else
                query = query.OrderBy("ETA desc");


            return query.Select(r => new T()
            {
                OrderNo = r.OrderNo,
                FactoryId = r.FactoryID,
                BlanketOrderNo = r.BlanketOrderNo,
                CreatedDate = r.CreatedDate,
                SupplierId = r.SupplierId,
                SupplierName = r.SupplierName,
                ETA = r.ETA,
                EDIDate = r.EDIDate
            });
        }

        public IQueryable<T> GetEStockTransferList<T>(EStockTransferListQueryFilter filter) where T : EStockTransferListQueryResult, new()
        {
            var query = from header in dbContext.EStockTransferHeaders
                        join customer in dbContext.Customers on header.FactoryID equals customer.Code
                        let detail = (from d in dbContext.EStockTransferDetails
                                      where d.OrderNo == header.OrderNo
                                      orderby d.SupplierID
                                      select d).FirstOrDefault()
                        join supplier in dbContext.SupplierMasters on
                        new { detail.SupplierID, header.FactoryID } equals new { supplier.SupplierID, supplier.FactoryID }
                        where header.Status == EStockTransferStatus.New && customer.WHSCode == filter.UserWHSCode
                        select new
                        {
                            header.RunNo,
                            header.StockTransferJobNo,
                            header.Status,
                            header.ETA,
                            header.OrderNo,
                            header.FactoryID,
                            header.BlanketOrderNo,
                            header.CreatedDate,
                            header.IssuedDate,
                            header.ConfirmationDate,
                            header.Instructions,
                            header.RefNo,
                            SupplierId = detail != null ? detail.SupplierID : null,
                            CompanyName = supplier.CompanyName
                        };

            if (!string.IsNullOrWhiteSpace(filter.FactoryID))
            {
                query = query.Where(h => h.FactoryID.ToLower().Contains(filter.FactoryID));
            }
            if (!string.IsNullOrWhiteSpace(filter.SupplierID))
            {
                query = query.Where(h => h.SupplierId.ToLower().Contains(filter.SupplierID));
            }
            if (!string.IsNullOrWhiteSpace(filter.CompanyName))
            {
                query = query.Where(h => h.CompanyName.ToLower().Contains(filter.CompanyName));
            }
            if (!string.IsNullOrWhiteSpace(filter.OrderNo))
            {
                query = query.Where(h => h.OrderNo.ToLower().Contains(filter.OrderNo.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(filter.BlanketOrderNo))
            {
                query = query.Where(h => h.BlanketOrderNo.ToLower().Contains(filter.BlanketOrderNo.ToLower()));
            }
            if (filter.ETAFrom.HasValue)
            {
                var date = filter.ETAFrom.Value.Date;
                query = query.Where(h => h.ETA.HasValue && h.ETA >= date);
            }
            if (filter.ETATo.HasValue)
            {
                var date = filter.ETATo.Value.Date.AddDays(1);
                query = query.Where(h => h.ETA.HasValue && h.ETA < date);
            }

            if (filter.OrderBy != null)
            {
                query = query.OrderBy(filter.OrderBy + (filter.Desc ? " desc" : " asc"));
            }
            else
                query = query.OrderBy("ETA desc");

            return query.Select(r => new T()
            {
                OrderNo = r.OrderNo,
                BlanketOrderNo = r.BlanketOrderNo,
                CreatedDate = r.CreatedDate,
                SupplierID = r.SupplierId,
                FactoryID = r.FactoryID,
                RunNo = r.RunNo,
                IssuedDate = r.IssuedDate,
                ConfirmationDate = r.ConfirmationDate,
                Instructions = r.Instructions,
                Status = r.Status,
                StockTransferJobNo = r.StockTransferJobNo,
                ETA = r.ETA,
                RefNo = r.RefNo,
                CompanyName = r.CompanyName
            });
        }


        public IQueryable<T> GetPartMasterList<T>(PartMasterListQueryFilter filter) where T : PartMasterListQueryResult, new()
        {
            var query = from pm in dbContext.PartMasters
                        join uom in dbContext.UOMs on pm.UOM equals uom.Code
                        where pm.CustomerCode == filter.CustomerCode
                        select new
                        {
                            pm,
                            uom
                        };
            if (!string.IsNullOrWhiteSpace(filter.SupplierID))
            {
                query = query.Where(o => o.pm.SupplierID.ToLower().Contains(filter.SupplierID.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(filter.ProductCode1))
            {
                query = query.Where(o => o.pm.ProductCode1.ToLower().Contains(filter.ProductCode1.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(filter.ProductCode2))
            {
                query = query.Where(o => o.pm.ProductCode2.ToLower().Contains(filter.ProductCode2.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(filter.Description))
            {
                query = query.Where(o => o.pm.Description.ToLower().Contains(filter.Description.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(filter.UomName))
            {
                query = query.Where(o => o.uom.Name.ToLower().Contains(filter.UomName.ToLower()));
            }
            if (filter.Statuses != null && filter.Statuses.Any())
            {
                query = query.Where(o => filter.Statuses.Contains((ValueStatus)o.pm.Status));
            }
            if (filter.IsDefected.HasValue)
            {
                query = query.Where(o => o.pm.IsDefected == 1 == filter.IsDefected.Value);
            }
            if (!string.IsNullOrWhiteSpace(filter.iLogReadinessStatus))
            {
                query = query.Where(o => o.pm.iLogReadinessStatus == filter.iLogReadinessStatus);
            }

            if (filter.OrderBy != null)
            {
                switch (filter.OrderBy)
                {
                    case "supplierID":
                        query = filter.Desc ?
                            query.OrderByDescending(r => r.pm.SupplierID).ThenBy(r => r.pm.CustomerCode).ThenBy(r => r.pm.ProductCode1) :
                            query.OrderBy(r => r.pm.SupplierID).ThenBy(r => r.pm.CustomerCode).ThenBy(r => r.pm.ProductCode1);
                        break;
                    case "customerCode":
                        query = filter.Desc ? query.OrderByDescending(r => r.pm.CustomerCode) : query.OrderBy(r => r.pm.CustomerCode);
                        break;
                    case "productCode1":
                        query = filter.Desc ? query.OrderByDescending(r => r.pm.ProductCode1) : query.OrderBy(r => r.pm.ProductCode1);
                        break;
                    case "productCode2":
                        query = filter.Desc ? query.OrderByDescending(r => r.pm.ProductCode2) : query.OrderBy(r => r.pm.ProductCode2);
                        break;
                    case "description":
                        query = filter.Desc ? query.OrderByDescending(r => r.pm.Description) : query.OrderBy(r => r.pm.Description);
                        break;
                    case "isDefected":
                        query = filter.Desc ? query.OrderByDescending(r => r.pm.IsDefected) : query.OrderBy(r => r.pm.IsDefected);
                        break;
                    case "spq":
                        query = filter.Desc ? query.OrderByDescending(r => r.pm.SPQ) : query.OrderBy(r => r.pm.SPQ);
                        break;
                    case "statusString":
                        query = filter.Desc ? query.OrderByDescending(r => r.pm.Status) : query.OrderBy(r => r.pm.Status);
                        break;
                    case "uom":
                        query = filter.Desc ? query.OrderByDescending(r => r.pm.UOM) : query.OrderBy(r => r.pm.UOM);
                        break;
                    case "uomName":
                        query = filter.Desc ? query.OrderByDescending(r => r.uom.Name) : query.OrderBy(r => r.uom.Name);
                        break;
                    case "iLogReadinessStatus":
                        query = filter.Desc ? query.OrderByDescending(r => r.pm.iLogReadinessStatus) : query.OrderBy(r => r.pm.iLogReadinessStatus);
                        break;
                    default:
                        query = query.OrderBy(r => r.pm.SupplierID).ThenBy(r => r.pm.CustomerCode).ThenBy(r => r.pm.ProductCode1);
                        break;
                }
            }
            else
                query = query.OrderBy(r => r.pm.SupplierID).ThenBy(r => r.pm.CustomerCode).ThenBy(r => r.pm.ProductCode1);

            return query.Select(r => new T()
            {
                SupplierID = r.pm.SupplierID,
                CustomerCode = r.pm.CustomerCode,
                ProductCode1 = r.pm.ProductCode1,
                ProductCode2 = r.pm.ProductCode2,
                Description = r.pm.Description,
                IsDefected = r.pm.IsDefected == 1,
                SPQ = r.pm.SPQ,
                Status = (ValueStatus)r.pm.Status,
                UOM = r.pm.UOM,
                UOMName = r.uom.Name,
                iLogReadinessStatus = r.pm.iLogReadinessStatus
            });
        }

        public IQueryable<T> GetUsersList<T>(UserListQueryFilter filter) where T : UserListQueryResult, new()
        {
            var query = dbContext.Users.AsQueryable();
            if (!string.IsNullOrEmpty(filter.Code))
            {
                query = query.Where(u => u.Code.ToLower().Contains(filter.Code.ToLower()));
            }
            if (!string.IsNullOrEmpty(filter.FirstName))
            {
                query = query.Where(u => u.FirstName.ToLower().Contains(filter.FirstName.ToLower()));
            }
            if (!string.IsNullOrEmpty(filter.LastName))
            {
                query = query.Where(u => u.LastName.ToLower().Contains(filter.LastName.ToLower()));
            }
            if (!string.IsNullOrEmpty(filter.GroupCode))
            {
                query = query.Where(u => u.GroupCode.ToLower().Contains(filter.GroupCode.ToLower()));
            }
            if (!string.IsNullOrEmpty(filter.WHSCode))
            {
                query = query.Where(u => u.WHSCode == filter.WHSCode);
            }
            if (filter.Status.HasValue)
            {
                query = query.Where(u => u.Status == filter.Status);
            }
            switch (filter.OrderBy)
            {
                case "firstName":
                    query = filter.Desc ? query.OrderByDescending(r => r.FirstName) : query.OrderBy(r => r.FirstName);
                    break;
                case "lastName":
                    query = filter.Desc ? query.OrderByDescending(r => r.LastName) : query.OrderBy(r => r.LastName);
                    break;
                case "groupCode":
                    query = filter.Desc ? query.OrderByDescending(r => r.GroupCode) : query.OrderBy(r => r.GroupCode);
                    break;
                case "status":
                    query = filter.Desc ? query.OrderByDescending(r => r.Status) : query.OrderBy(r => r.Status);
                    break;
                case "whsCode":
                    query = filter.Desc ? query.OrderByDescending(r => r.WHSCode) : query.OrderBy(r => r.WHSCode);
                    break;
                case "code":
                default:
                    query = filter.Desc ? query.OrderByDescending(r => r.Code) : query.OrderBy(r => r.Code);
                    break;
            }

            return query.Select(r => new T
            {
                Code = r.Code,
                FirstName = r.FirstName,
                LastName = r.LastName,
                GroupCode = r.GroupCode,
                Status = r.Status,
                WHSCode = r.WHSCode
            });
        }

        public IQueryable<T> GetLoadingList<T>(LoadingListQueryFilter filter) where T : LoadingListQueryResult, new()
        {
            var query = from loading in dbContext.Loadings
                        join customer in dbContext.Customers on loading.CustomerCode equals customer.Code
                        where loading.WHSCode == filter.UserWHSCode && customer.WHSCode == filter.UserWHSCode
                        select new
                        {
                            loading,
                            customer,
                            minStatus = (from ob in dbContext.Outbounds
                                         join ek in dbContext.EKanbanHeaders on ob.JobNo equals ek.OutJobNo
                                         join ld in dbContext.LoadingDetails on ek.OrderNo equals ld.OrderNo
                                         where ld.JobNo == loading.JobNo
                                         select (int?)ob.Status
                                             ).Min() ?? 0,
                            maxStatus = (from ob in dbContext.Outbounds
                                         join ek in dbContext.EKanbanHeaders on ob.JobNo equals ek.OutJobNo
                                         join ld in dbContext.LoadingDetails on ek.OrderNo equals ld.OrderNo
                                         where ld.JobNo == loading.JobNo
                                         select (int?)ob.Status
                                             ).Max() ?? 0,
                            noOfPallet = (from ob in dbContext.Outbounds
                                          join ld in dbContext.LoadingDetails on ob.JobNo equals ld.OutJobNo
                                          join pl in dbContext.PickingLists on ob.JobNo equals pl.JobNo
                                          where ld.JobNo == loading.JobNo
                                          select new { pl.JobNo, pl.LineItem }).Count()
                        };


            if (!String.IsNullOrWhiteSpace(filter.JobNo))
            {
                query = query.Where(o => o.loading.JobNo.ToLower().Contains(filter.JobNo.ToLower()));
            }
            if (filter.CustomerCodes?.Any() == true)
            {
                var customerCodes = filter.CustomerCodes.Select(c => c.ToLower());
                query = query.Where(o => customerCodes.Contains(o.customer.Code.ToLower()));
            }
            if (!String.IsNullOrWhiteSpace(filter.RefNo))
            {
                query = query.Where(o => o.loading.RefNo.ToLower().Contains(filter.RefNo.ToLower()));
            }
            if (filter.ETD.HasValue)
            {
                var date = filter.ETD.Value.Date;
                query = query.Where(o => o.loading.ETD.HasValue && o.loading.ETD.Value.Date == date);
            }
            if (filter.ETA.HasValue)
            {
                var date = filter.ETA.Value.Date;
                query = query.Where(o => o.loading.ETA.HasValue && o.loading.ETA.Value.Date == date);
            }
            if (filter.CreatedDate.HasValue)
            {
                var date = filter.CreatedDate.Value.Date;
                query = query.Where(o => o.loading.CreatedDate.Date == date);
            }
            if (filter.TruckArrivalDate.HasValue)
            {
                var date = filter.TruckArrivalDate.Value.Date;
                query = query.Where(o => o.loading.TruckArrivalDate.HasValue && o.loading.TruckArrivalDate.Value.Date == date);
            }
            if (filter.TruckDepartureDate.HasValue)
            {
                var date = filter.TruckDepartureDate.Value.Date;
                query = query.Where(o => o.loading.TruckDepartureDate.HasValue && o.loading.TruckDepartureDate.Value.Date == date);
            }
            if (!String.IsNullOrWhiteSpace(filter.Remark))
            {
                if (!String.IsNullOrWhiteSpace(filter.Remark))
                {
                    switch (filter.RemarkFilter ?? StringFilterMode.Contains)
                    {
                        case StringFilterMode.Equals:
                            query = query.Where(o => o.loading.Remark.ToLower().Equals(filter.Remark.ToLower()));
                            break;
                        case StringFilterMode.StartsWith:
                            query = query.Where(o => o.loading.Remark.ToLower().StartsWith(filter.Remark.ToLower()));
                            break;
                        case StringFilterMode.EndsWith:
                            query = query.Where(o => o.loading.Remark.ToLower().EndsWith(filter.Remark.ToLower()));
                            break;
                        case StringFilterMode.Contains:
                        default:
                            query = query.Where(o => o.loading.Remark.ToLower().Contains(filter.Remark.ToLower()));
                            break;
                    }
                }
            }
            if (filter.Statuses != null && filter.Statuses.Any())
            {
                query = query.Where(o => filter.Statuses.Contains(o.loading.Status));
            }
            if (!String.IsNullOrWhiteSpace(filter.TruckLicencePlate))
            {
                query = query.Where(o => o.loading.TruckLicencePlate.ToLower().Contains(filter.TruckLicencePlate.ToLower()));
            }
            if (!String.IsNullOrWhiteSpace(filter.TrailerNo))
            {
                query = query.Where(o => o.loading.TrailerNo.ToLower().Contains(filter.TrailerNo.ToLower()));
            }
            if (!String.IsNullOrWhiteSpace(filter.DockNo))
            {
                query = query.Where(o => o.loading.DockNo.ToLower().Contains(filter.DockNo.ToLower()));
            }
            if (!String.IsNullOrWhiteSpace(filter.TruckSeqNo))
            {
                query = query.Where(o => o.loading.TruckSeqNo.ToLower().Contains(filter.TruckSeqNo.ToLower()));
            }
            if (filter.AllowedForDispatch.HasValue)
            {
                query = query.Where(o => o.loading.AllowedForDispatch == filter.AllowedForDispatch.Value);
            }

            if (filter.OrderBy != null)
            {
                switch (filter.OrderBy)
                {
                    case "jobNo":
                    case "refNo":
                    case "etd":
                    case "eta":
                    case "createdDate":
                    case "truckArrivalDate":
                    case "truckDepartureDate":
                    case "remark":
                    case "status":
                    case "truckLicencePlate":
                    case "truckSeqNo":
                    case "trailerNo":
                    case "dockNo":
                    case "allowedForDispatch":
                        query = query.OrderBy("loading." + filter.OrderBy + (filter.Desc ? " desc" : " asc"));
                        break;
                    case "customerName":
                        query = query.OrderBy("customer.Name" + (filter.Desc ? " desc" : " asc"));
                        break;
                    case "customerCode":
                        query = query.OrderBy("customer.Code" + (filter.Desc ? " desc" : " asc"));
                        break;
                    case "calculatedStatusString":
                        query = filter.Desc ? query.OrderByDescending(s => s.minStatus).ThenByDescending(s => s.maxStatus) : query.OrderBy(s => s.minStatus).ThenBy(s => s.maxStatus);
                        break;
                    case "noOfPallet":
                        query = filter.Desc ? query.OrderByDescending(s => s.noOfPallet) : query.OrderBy(s => s.noOfPallet);
                        break;
                    default:
                        break;
                }
            }
            else
                query = query.OrderBy("loading.JobNo asc");

            return query.Select(r => new T()
            {
                JobNo = r.loading.JobNo,
                CustomerName = r.customer.Name,
                CustomerCode = r.customer.Code,
                TruckArrivalDate = r.loading.TruckArrivalDate,
                TruckDepartureDate = r.loading.TruckDepartureDate,
                RefNo = r.loading.RefNo,
                WHSCode = r.loading.WHSCode,
                CreatedDate = r.loading.CreatedDate,
                ETA = r.loading.ETA,
                ETD = r.loading.ETD,
                Status = Enum.Parse<LoadingStatus>(r.loading.Status.ToString()),
                MinOutboundStatus = Enum.Parse<OutboundStatus>(r.minStatus.ToString()),
                MaxOutboundStatus = Enum.Parse<OutboundStatus>(r.maxStatus.ToString()),
                Remark = r.loading.Remark,
                TruckLicencePlate = r.loading.TruckLicencePlate,
                TruckSeqNo = r.loading.TruckSeqNo,
                TrailerNo = r.loading.TrailerNo,
                DockNo = r.loading.DockNo,
                AllowedForDispatch = r.loading.AllowedForDispatch,
                NoOfPallet = r.loading.NoOfPallet.HasValue && r.loading.NoOfPallet > 0 ? r.loading.NoOfPallet.Value : r.noOfPallet
            });
        }

        public IQueryable<T> GetInboundList<T>(InboundListQueryFilter filter) where T : InboundListQueryResult, new()
        {
            var query = from inbound in dbContext.Inbounds
                        join customer in dbContext.Customers on new { Code = inbound.CustomerCode, inbound.WHSCode }
                            equals new { customer.Code, customer.WHSCode }
                        join sms in dbContext.SupplierMasters on new { inbound.SupplierID, inbound.CustomerCode }
                            equals new { sms.SupplierID, CustomerCode = sms.FactoryID } into supmas
                        from sm in supmas.DefaultIfEmpty()
                        where inbound.WHSCode == filter.UserWHSCode && customer.WHSCode == filter.UserWHSCode
                        select new
                        {
                            inbound,
                            customer,
                            supplierName = sm.CompanyName,
                            containerNos = (from asnHeader in dbContext.ASNHeaders
                                            join id in dbContext.InboundDetails on asnHeader.ASNNo equals id.ASNNo
                                            where id.JobNo == inbound.JobNo
                                            select asnHeader.ContainerNoEU).ToList()
                        };

            if (!String.IsNullOrWhiteSpace(filter.JobNo))
            {
                query = query.Where(o => o.inbound.JobNo.ToLower().Contains(filter.JobNo.ToLower()));
            }
            if (filter.CustomerCodes?.Any() == true)
            {
                var customerCodes = filter.CustomerCodes.Select(c => c.ToLower());
                query = query.Where(o => customerCodes.Contains(o.customer.Code.ToLower()));
            }
            if (!String.IsNullOrWhiteSpace(filter.CustomerName))
            {
                query = query.Where(o => o.customer.Name.ToLower().Contains(filter.CustomerName.ToLower()));
            }
            if (!String.IsNullOrWhiteSpace(filter.RefNo))
            {
                query = query.Where(o => o.inbound.RefNo.ToLower().Contains(filter.RefNo.ToLower()));
            }
            if (!String.IsNullOrWhiteSpace(filter.SupplierName))
            {
                query = query.Where(o => o.supplierName.ToLower().Contains(filter.SupplierName.ToLower()));
            }
            if (!String.IsNullOrWhiteSpace(filter.ASNNumber))
            {
                query = query.Where(o => o.inbound.IRNo.ToLower().Contains(filter.ASNNumber.ToLower()));
            }
            if (!String.IsNullOrWhiteSpace(filter.ContainerNo))
            {
                query = query.Where(o => o.containerNos.Any(c => c.ToLower().Contains(filter.ContainerNo.ToLower())));
            }
            if (filter.TransType.HasValue)
            {
                query = query.Where(o => o.inbound.TransType == filter.TransType.Value);
            }
            if (filter.ReceivedDate.HasValue)
            {
                var date = filter.ReceivedDate.Value.Date;
                query = query.Where(o => o.inbound.ETA.Date == date);
            }
            query = filter.Remark switch
            {
                _ when string.IsNullOrWhiteSpace(filter.Remark) => query,
                _ when filter.Remark.Contains('%') => query.Where(i => EF.Functions.Like(i.inbound.Remark, FormatForWildcardSearch(filter.Remark), EFCoreExtensions.ESCAPE_CHAR)),
                _ => query.Where(i => i.inbound.Remark == filter.Remark),
            };
            if (filter.Statuses != null && filter.Statuses.Any())
            {
                //var intStatuses = filter.Statuses.Select(s => (int)s);
                query = query.Where(o => filter.Statuses.Contains(o.inbound.Status));
            }

            if (filter.OrderBy != null)
            {
                switch (filter.OrderBy)
                {
                    case "jobNo":
                    case "refNo":
                    case "eta":
                    case "transType":
                    case "remark":
                    case "createdDate":
                    case "status":
                        query = query.OrderBy("inbound." + filter.OrderBy + (filter.Desc ? " desc" : " asc"));
                        break;
                    case "statusString":
                        query = query.OrderBy("inbound.status" + (filter.Desc ? " desc" : " asc"));
                        break;
                    case "transTypeString":
                        query = query.OrderBy("inbound.TransType" + (filter.Desc ? " desc" : " asc"));
                        break;
                    case "asnNumber":
                        query = query.OrderBy("inbound.IRNo" + (filter.Desc ? " desc" : " asc"));
                        break;
                    case "receivedDate":
                        query = query.OrderBy("inbound.ETA" + (filter.Desc ? " desc" : " asc"));
                        break;
                    case "customerName":
                        query = query.OrderBy("customer.Name" + (filter.Desc ? " desc" : " asc"));
                        break;
                    case "customerCode":
                        query = query.OrderBy("customer.Code" + (filter.Desc ? " desc" : " asc"));
                        break;
                    case "supplierName":
                        query = filter.Desc ? query.OrderByDescending(i => i.supplierName) : query.OrderBy(i => i.supplierName);
                        break;
                    default:
                        break;
                    case "containerNo":
                        query = filter.Desc ? query.OrderByDescending(q => q.containerNos.First()) : query.OrderBy(q => q.containerNos.First());
                        break;

                }
            }
            else
                query = query.OrderBy("inbound.JobNo asc");

            return query.Select(r => new T()
            {
                JobNo = r.inbound.JobNo,
                CustomerName = r.customer.Name,
                CustomerCode = r.customer.Code,
                RefNo = r.inbound.RefNo,
                WHSCode = r.inbound.WHSCode,
                SupplierName = r.supplierName,
                ASNNumber = r.inbound.IRNo,
                ContainerNos = r.containerNos,
                TransType = r.inbound.TransType,
                ReceivedDate = r.inbound.ETA,
                Status = r.inbound.Status,
                Remark = r.inbound.Remark
            });
        }

        public IQueryable<T> GetStorageDetailGroupList<T>(string whsCode, StorageGroupListQueryFilter filter) where T : StorageDetailGroup, new()
        {
            var query = (from gr in dbContext.StorageDetailGroups

                         select new
                         {
                             gr,
                             inbounds = (from sd in dbContext.StorageDetails
                                         where sd.GroupID == gr.GroupID
                                         select sd.InJobNo
                             ).ToList()
                         });
            query = query.Where(o => o.gr.WHSCode == whsCode);

            if (!String.IsNullOrWhiteSpace(filter.GroupID))
            {
                query = query.Where(o => o.gr.GroupID.ToLower().Contains(filter.GroupID.Trim().ToLower()));
            }
            if (filter.CreatedDate.HasValue)
            {
                query = query.Where(o => o.gr.CreatedDate.Date == filter.CreatedDate.Value.Date);
            }
            if (filter.RepackedDate.HasValue)
            {
                query = query.Where(o => o.gr.RepackedDate.HasValue && o.gr.RepackedDate.Value.Date == filter.RepackedDate.Value.Date);
            }
            if (filter.ClosedDate.HasValue)
            {
                query = query.Where(o => o.gr.ClosedDate.HasValue && o.gr.ClosedDate.Value.Date == filter.ClosedDate.Value.Date);
            }
            if (!String.IsNullOrWhiteSpace(filter.InJobNo))
            {
                query = query.Where(o => o.inbounds.Contains(filter.InJobNo.Trim().ToUpper()));
            }
            if (filter.Status.HasValue)
            {
                var status = filter.Status.Value;
                query = query.Where(o =>
                    (status == StorageGroupStatus.InUse && !o.gr.RepackedDate.HasValue && !o.gr.ClosedDate.HasValue) ||
                    (status == StorageGroupStatus.Closed && o.gr.ClosedDate.HasValue && (!o.gr.RepackedDate.HasValue || o.gr.ClosedDate > o.gr.RepackedDate)) ||
                    (status == StorageGroupStatus.Transformed && o.gr.RepackedDate.HasValue && (!o.gr.ClosedDate.HasValue || o.gr.ClosedDate <= o.gr.RepackedDate)));
            }

            if (filter.OrderBy != null)
            {
                switch (filter.OrderBy)
                {
                    case "groupID":
                    case "createdDate":
                    case "closedDate":
                    case "repackedDate":
                        query = query.OrderBy("gr." + filter.OrderBy + (filter.Desc ? " desc" : " asc"));
                        break;

                    default:
                        break;


                }
            }
            else
                query = query.OrderBy("gr.CreatedDate desc");

            return query.Select(r => new T()
            {
                GroupID = r.gr.GroupID,
                CreatedDate = r.gr.CreatedDate,
                RepackedDate = r.gr.RepackedDate,
                ClosedDate = r.gr.ClosedDate,
                Quantity = r.gr.Quantity,
                InJobNo = r.inbounds,
                Name = r.gr.Name
            });
        }


        public IQueryable<T> GetASNListToImport<T>(ASNListQueryFilter filter) where T : ASNListQueryResult, new()
        {
            var asnHeaderStatuses = new string[] { "NEW", "ARV", "CCD", "DLV", "PKD", "BKD", "A&C", "RCD", "DEP", "IMP" };
            var query = from asnHeader in dbContext.ASNHeaders
                        join sm in dbContext.SupplierMasters on new { asnHeader.SupplierID, asnHeader.FactoryID }
                            equals new { sm.SupplierID, sm.FactoryID }
                        join customer in dbContext.Customers on asnHeader.FactoryID equals customer.Code
                        join asnDetail in dbContext.ASNDetails on asnHeader.ASNNo equals asnDetail.ASNNo
                        where asnHeaderStatuses.Contains(asnHeader.Status)
                            && customer.WHSCode == filter.WHSCode
                            && String.IsNullOrEmpty(asnDetail.InJobNo)
                        select new T
                        {
                            AsnNo = asnHeader.ASNNo,
                            ContainerNo = asnHeader.ContainerNoEU,
                            FactoryID = asnHeader.FactoryID,
                            SupplierID = asnHeader.SupplierID,
                            IsVMI = asnHeader.IsVMI == 1,
                            SupplierName = sm.CompanyName
                        };
            query = query.Distinct();

            if (filter.OrderBy != null)
                query = query.OrderBy(filter.OrderBy + (filter.Desc ? " desc" : " asc"));
            else
                query = query.OrderBy("AsnNo asc");

            if (!String.IsNullOrWhiteSpace(filter.ASNNo))
            {
                query = query.Where(o => o.AsnNo.ToLower().Contains(filter.ASNNo.ToLower()));
            }
            if (!String.IsNullOrWhiteSpace(filter.ContainerNo))
            {
                query = query.Where(o => o.ContainerNo.ToLower().Contains(filter.ContainerNo.ToLower()));
            }
            if (!String.IsNullOrWhiteSpace(filter.FactoryID))
            {
                query = query.Where(o => o.FactoryID.ToLower().Contains(filter.FactoryID.ToLower()));
            }
            if (!String.IsNullOrWhiteSpace(filter.SupplierID))
            {
                query = query.Where(o => o.SupplierID.ToLower().Contains(filter.SupplierID.ToLower()));
            }
            if (!String.IsNullOrWhiteSpace(filter.SupplierName))
            {
                query = query.Where(o => o.SupplierName.ToLower().Contains(filter.SupplierName.ToLower()));
            }
            if (filter.IsVMI.HasValue)
            {
                query = query.Where(o => o.IsVMI == filter.IsVMI);
            }

            return query;
        }

        public async Task<IEnumerable<OutstandingInboundForReportQueryResult>> GetOutstandingInboundForReport(string whsCode)
        {
            var statuses = new InboundStatus[] { InboundStatus.Completed, InboundStatus.Cancelled };
            return await dbContext.Inbounds.Where(i => !statuses.Contains(i.Status) && i.WHSCode == whsCode)
                .OrderBy(i => i.CreatedDate)
                .Select(i => new OutstandingInboundForReportQueryResult
                {
                    JobNo = i.JobNo,
                    ASN = i.IRNo,
                    CreatedDate = i.CreatedDate,
                    Status = i.Status
                }).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetPickingListWithUOM<T>(string jobNo, int? lineItem) where T : PickingListSimpleQueryResult, new()
        {
            var query = from pickingList in dbContext.PickingLists
                        join outbound in dbContext.Outbounds on pickingList.JobNo equals outbound.JobNo
                        join partMaster in dbContext.PartMasters
                            on new { pickingList.ProductCode, pickingList.SupplierID, outbound.CustomerCode }
                            equals new { ProductCode = partMaster.ProductCode1, partMaster.SupplierID, partMaster.CustomerCode }
                        join uom in dbContext.UOMs on partMaster.UOM equals uom.Code

                        join ud in dbContext.UOMDecimals
                            on new { partMaster.CustomerCode, partMaster.UOM, Status = (byte)1 }
                            equals new { ud.CustomerCode, ud.UOM, ud.Status } into uds
                        from uomdec in uds.DefaultIfEmpty()

                        join epid in dbContext.ExternalPIDs on pickingList.PID equals epid.PID into expid
                        from externalpid in expid.DefaultIfEmpty()

                        join sd in dbContext.StorageDetails on pickingList.PID equals sd.PID into sds
                        from storageDet in sds.DefaultIfEmpty()

                        join ib in dbContext.Inbounds on storageDet.InJobNo equals ib.JobNo into ibs
                        from inbound in ibs.DefaultIfEmpty()

                        join pm in dbContext.PriceMasters
                            on new { partMaster.CustomerCode, partMaster.SupplierID, partMaster.ProductCode1 }
                            equals new { pm.CustomerCode, pm.SupplierID, pm.ProductCode1 } into priceM
                        from priceMaster in priceM.DefaultIfEmpty()

                        where pickingList.JobNo == jobNo
                            && (!lineItem.HasValue || pickingList.LineItem == lineItem)
                        orderby pickingList.LineItem ascending, pickingList.SeqNo ascending

                        select new T()
                        {
                            JobNo = pickingList.JobNo,
                            LineItem = pickingList.LineItem,
                            SeqNo = pickingList.SeqNo,
                            ProductCode = pickingList.ProductCode,
                            SupplierID = pickingList.SupplierID,
                            Qty = pickingList.Qty,
                            PID = pickingList.PID,
                            WHSCode = pickingList.WHSCode,
                            LocationCode = pickingList.LocationCode,
                            InboundDate = pickingList.InboundDate,
                            InboundJobNo = storageDet != null ? storageDet.InJobNo : pickingList.InboundJobNo,
                            PickedBy = pickingList.PickedBy,
                            PickedDate = pickingList.PickedDate,
                            ExternalPID = externalpid != null ? externalpid.ExternalID : null,
                            DecimalNum = uomdec != null ? uomdec.DecimalNum : 0,
                            Ownership = storageDet != null ? (int)storageDet.Ownership : -1,
                            PIDValue = storageDet != null ?
                                            (storageDet.SellingPrice > 0 ? (pickingList.Qty * storageDet.SellingPrice) :
                                            (pickingList.Qty * storageDet.BuyingPrice)) : 0,
                            Currency = inbound.Currency,
                            Price = priceMaster != null ? priceMaster.SellingPrice : 0,
                            CustomerCode = outbound.CustomerCode
                        };

            return await query.ToListAsync();

            /*
						m_strSQL = "SELECT TT_PickingList.*, ISNULL(TT_UOMDecimal.DecimalNum,0) AS DecimalNum, " +
                            "ISNULL(TT_ExternalPID.ExternalPID,'') AS ExternalPID, ISNULL(CONVERT(int,Ownership),-1) AS Ownership, " +
                            "TT_StorageDetail.InJobNo, " +
                            "CASE WHEN ISNULL(TT_StorageDetail.SellingPrice,0) > 0 THEN TT_PickingList.Qty * ISNULL(TT_StorageDetail.SellingPrice,0) 
                            ELSE TT_PickingList.Qty * ISNULL(TT_StorageDetail.BuyingPrice, 0) END AS PIDValue, " +

                            "ISNULL(TT_Inbound.Currency,'') AS Currency, ISNULL(TT_PriceMaster.SellingPrice,0) AS Price, 
                            TT_Outbound.CustomerCode  " +
							"FROM   TT_PickingList " +
							"       INNER JOIN TT_Outbound ON TT_Outbound.JobNo = TT_PickingList.JobNo " + 
							"       INNER JOIN TT_PartMaster ON TT_PickingList.ProductCode = TT_PartMaster.ProductCode1 AND " + 
							"                  TT_Outbound.CustomerCode = TT_PartMaster.CustomerCode  AND  " +
							"                  TT_PickingList.SupplierID = TT_PartMaster.SupplierID " +
							"       INNER JOIN TT_UOM ON TT_PartMaster.UOM = TT_UOM.Code " + 
							"       LEFT OUTER JOIN TT_UOMDecimal ON " + 
							"       TT_UOMDecimal.CustomerCode = TT_PartMaster.CustomerCode AND " + 
							"       TT_UOMDecimal.UOM = TT_PartMaster.UOM AND TT_UOMDecimal.Status = 1 " + 
                            "       LEFT JOIN TT_ExternalPID ON " +
                            "       TT_PickingList.PID = TT_ExternalPID.PID " +
                            "       LEFT JOIN TT_StorageDetail ON " +
                            "       TT_PickingList.PID = TT_StorageDetail.PID " +
                            "       LEFT JOIN TT_Inbound ON " +
                            "       TT_StorageDetail.InJobNo = TT_Inbound.JobNo " +

                            "       LEFT JOIN TT_PriceMaster ON " +
                            "       TT_PartMaster.CustomerCode = TT_PriceMaster.CustomerCode " +
                            "       AND TT_PartMaster.SupplierID = TT_PriceMaster.SupplierID " +
                            "       AND TT_PartMaster.ProductCode1 = TT_PriceMaster.ProductCode1 " +
							p_oFilter.GetFilterState();
            */
        }

        public async Task<IEnumerable<T>> GetOutboundPickableList<T>(OutboundPickableListQueryFilter filter) where T : OutboundPickableListQueryResult, new()
        {
            if (filter.OutboundTransType == OutboundType.Return)
            {
                return await (from pm in dbContext.PartMasters
                              join inv in dbContext.Inventory on new { pm.CustomerCode, pm.ProductCode1, pm.SupplierID }
                              equals new { inv.CustomerCode, inv.ProductCode1, inv.SupplierID }
                              where pm.CustomerCode == filter.CustomerCode
                                    && pm.SupplierID == filter.SupplierID
                                    && inv.WHSCode == filter.WHSCode
                                    && pm.Status == (int)filter.PartMasterStatus
                                    && inv.Ownership == (byte)Ownership.Supplier
                              select new T()
                              {
                                  SupplierID = pm.SupplierID,
                                  CustomerCode = pm.CustomerCode,
                                  ProductCode1 = pm.ProductCode1,
                                  ProductCode2 = pm.ProductCode2,
                                  ProductCode3 = pm.ProductCode3,
                                  Description = pm.Description,
                                  IsStandardPackaging = pm.IsStandardPackaging,
                                  UOM = pm.UOM,
                                  SPQ = pm.SPQ,
                                  IsCPart = pm.IsCPart == 1,
                                  CPartSPQ = pm.CPartSPQ,
                                  OnHandQty = inv.OnHandQty - inv.AllocatedQty - inv.QuarantineQty,
                                  EHPQty = 0,
                                  SupplierQty = inv.OnHandQty - inv.AllocatedQty - inv.QuarantineQty
                              }).ToListAsync();
            }
            else
            {
                var data = new List<T>();
                var dbresult = await (from pm in dbContext.PartMasters
                                      join inv in dbContext.Inventory on new { pm.CustomerCode, pm.ProductCode1, pm.SupplierID }
                                      equals new { inv.CustomerCode, inv.ProductCode1, inv.SupplierID }
                                      where pm.CustomerCode == filter.CustomerCode
                                            && pm.SupplierID == filter.SupplierID
                                            && inv.WHSCode == filter.WHSCode
                                            && pm.Status == (int)filter.PartMasterStatus
                                      select new { pm, inv }
                                    ).ToListAsync();
                var result = dbresult.GroupBy(g =>
                    new { g.pm.SupplierID, g.pm.CustomerCode, g.pm.ProductCode1, g.pm.ProductCode2, g.pm.ProductCode3, g.pm.Description, g.pm.IsStandardPackaging, g.pm.UOM, g.pm.SPQ, IsCPart = g.pm.IsCPart == 1, g.pm.CPartSPQ })
                    .Select(grouping => new
                    {
                        grouping.Key.SupplierID,
                        grouping.Key.CustomerCode,
                        grouping.Key.ProductCode1,
                        grouping.Key.ProductCode2,
                        grouping.Key.ProductCode3,
                        grouping.Key.Description,
                        grouping.Key.IsStandardPackaging,
                        grouping.Key.UOM,
                        grouping.Key.SPQ,
                        grouping.Key.IsCPart,
                        grouping.Key.CPartSPQ,
                        PartMasterOnHandQty = grouping.Sum(v => v.inv.OnHandQty),
                        PartMasterOnHandPkg = grouping.Where(v => v.inv.OnHandPkg.HasValue).Sum(v => v.inv.OnHandPkg.Value),
                        PartMasterAllocatedQty = grouping.Sum(v => v.inv.AllocatedQty),
                        PartMasterAllocatedPkg = grouping.Where(v => v.inv.AllocatedPkg.HasValue).Sum(v => v.inv.AllocatedPkg.Value),
                        PartMasterQuarantineQty = grouping.Sum(v => v.inv.QuarantineQty),
                        PartMasterQuarantinePkg = grouping.Where(v => v.inv.QuarantinePkg.HasValue).Sum(v => v.inv.QuarantinePkg.Value)
                    }).ToList();

                var productCodes = result.Select(r => r.ProductCode1).ToList();
                var pickableQties = dbContext.StorageDetails.Where(sd =>
                               sd.LocationCode == Enum.GetName(typeof(ExtSystemLocation), ExtSystemLocation.RETURN)
                               && sd.CustomerCode == filter.CustomerCode
                               && sd.SupplierID == filter.SupplierID
                               && productCodes.Contains(sd.ProductCode))
                    .Select(i => new { i.ProductCode, i.Qty, i.AllocatedQty })
                    .ToList()
                    .GroupBy(s => s.ProductCode)
                    .ToDictionary(g => g.Key, i => i.Sum(sd => sd.Qty - sd.AllocatedQty));

                var ehpStockQties = dbContext.Inventory.Where(i =>
                              i.Ownership == Ownership.EHP
                              && i.CustomerCode == filter.CustomerCode
                              && i.SupplierID == filter.SupplierID
                              && productCodes.Contains(i.ProductCode1)
                              && i.WHSCode == filter.WHSCode)
                    .Select(i => new { i.ProductCode1, Qty = i.OnHandQty - i.AllocatedQty - i.QuarantineQty })
                    .ToList()
                    .GroupBy(s => s.ProductCode1)
                    .ToDictionary(g => g.Key, i => i.Sum(sd => sd.Qty));

                var supplierQties = dbContext.Inventory.Where(i =>
                                i.Ownership == Ownership.Supplier
                                && i.CustomerCode == filter.CustomerCode
                                && i.SupplierID == filter.SupplierID
                                && productCodes.Contains(i.ProductCode1)
                                && i.WHSCode == filter.WHSCode)
                    .Select(i => new { i.ProductCode1, Qty = i.OnHandQty - i.AllocatedQty - i.QuarantineQty })
                    .ToList()
                    .GroupBy(s => s.ProductCode1)
                    .ToDictionary(g => g.Key, i => i.Sum(sd => sd.Qty));

                foreach (var row in result)
                {
                    var divideQty = row.IsCPart ? row.CPartSPQ : row.SPQ;
                    if (divideQty == 0) divideQty = 1;
                    if (pickableQties.TryGetValue(row.ProductCode1, out decimal pickableQty))
                    {
                        pickableQty = Math.Floor(pickableQty / divideQty);
                    }
                    ehpStockQties.TryGetValue(row.ProductCode1, out decimal ehpStockQty);

                    supplierQties.TryGetValue(row.ProductCode1, out decimal supplierQty);

                    var item = new T()
                    {
                        SupplierID = row.SupplierID,
                        CustomerCode = row.CustomerCode,
                        ProductCode1 = row.ProductCode1,
                        ProductCode2 = row.ProductCode2,
                        ProductCode3 = row.ProductCode3,
                        Description = row.Description,
                        IsStandardPackaging = row.IsStandardPackaging,
                        UOM = row.UOM,
                        SPQ = row.SPQ,
                        IsCPart = row.IsCPart,
                        CPartSPQ = row.CPartSPQ,
                        OnHandQty = row.PartMasterOnHandQty - row.PartMasterAllocatedQty - row.PartMasterQuarantineQty + pickableQty,
                        EHPQty = ehpStockQty - pickableQty,
                        SupplierQty = supplierQty
                    };
                    if (item.OnHandQty > 0 || !filter.OnlyOnHand)
                    {
                        data.Add(item);
                    }
                }
                return data.OrderBy(i => i.CustomerCode).ThenBy(i => i.ProductCode1);
            }

            /*
                    case ETT_OutboundGetListMethod.GetOutboundDetailPickableListManual:
            m_strSQL = "SELECT TT_PartMaster.CustomerCode, TT_PartMaster.SupplierID, TT_PartMaster.ProductCode1, " +
                "TT_PartMaster.ProductCode2, TT_PartMaster.ProductCode3, Description, SPQ, IsStandardPackaging, UOM, " +
                "SUM(OnHandQty - AllocatedQty - QuarantineQty) + ISNULL(ExtSD.PickableQty,0) as OnHandQty, 
                "ISNULL(EHPStock.EHPQty, 0) + ISNULL(ExtSD.PickableQty,0) as EHPQty, 
                ISNULL(SupplierStock.SupplierQty, 0) SupplierQty " +
                "FROM TT_PartMaster INNER JOIN TT_Inventory ON TT_PartMaster.CustomerCode = TT_Inventory.CustomerCode AND TT_PartMaster.ProductCode1 = TT_Inventory.ProductCode1 AND TT_PartMaster.SupplierID = TT_Inventory.SupplierID " +
              
                "LEFT JOIN (SELECT TT_StorageDetail.CustomerCode, TT_StorageDetail.SupplierID, ProductCode, " +
                "FLOOR(SUM(TT_StorageDetail.Qty - TT_StorageDetail.AllocatedQty)/CASE WHEN IsCPart = 1 THEN CPartSPQ ELSE SPQ END) * CASE WHEN IsCPart = 1 THEN CPartSPQ ELSE SPQ END as PickableQty " +
                "FROM TT_StorageDetail " +
                "INNER JOIN TT_PartMaster " +
                "ON TT_StorageDetail.CustomerCode = TT_PartMaster.CustomerCode " +
                "AND TT_StorageDetail.SupplierID = TT_PartMaster.SupplierID " +
                "AND TT_StorageDetail.ProductCode = TT_PartMaster.ProductCode1 " +
                "WHERE LocationCode = '" + Enum.GetName(typeof(EExtSystemLocation), (int)EExtSystemLocation.RETURN) + "' " +
                "GROUP BY TT_StorageDetail.CustomerCode, TT_StorageDetail.SupplierID, ProductCode, CPartSPQ, SPQ, IsCPart) ExtSD " +
                "ON TT_Inventory.CustomerCode = ExtSD.CustomerCode AND TT_Inventory.SupplierID = ExtSD.SupplierID " +
                "AND TT_Inventory.ProductCode1 = ExtSD.ProductCode " +

                "LEFT OUTER JOIN (SELECT EHPInv.CustomerCode, EHPInv.ProductCode1,EHPInv.SupplierID, EHPInv.WHSCode, " +
                "SUM(OnHandQty - AllocatedQty - QuarantineQty) as EHPQty, EHPInv.Ownership " +
                "FROM   TT_Inventory EHPInv  " +
                "WHERE EHPInv.Ownership = " + (int)EOwnership.EHP + " " +
                "GROUP BY EHPInv.CustomerCode, EHPInv.ProductCode1, EHPInv.WHSCode,EHPInv.SupplierID, EHPInv.Ownership) EHPStock " +
                "ON TT_Inventory.CustomerCode = EHPStock.CustomerCode AND TT_Inventory.SupplierID = EHPStock.SupplierID " +
                "AND TT_Inventory.ProductCode1 = EHPStock.ProductCode1 AND TT_Inventory.WHSCode = EHPStock.WHSCode " +

                "LEFT OUTER JOIN (SELECT SupInv.CustomerCode, SupInv.ProductCode1,SupInv.SupplierID, SupInv.WHSCode, " +
                "SUM(OnHandQty - AllocatedQty - QuarantineQty) AS SupplierQty, SupInv.Ownership " +
                "FROM   TT_Inventory SupInv  " +
                "WHERE SupInv.Ownership = " + (int)EOwnership.Supplier + " " +
                "GROUP BY SupInv.CustomerCode, SupInv.ProductCode1, SupInv.WHSCode,SupInv.SupplierID, SupInv.Ownership) SupplierStock " +
                "ON TT_Inventory.CustomerCode = SupplierStock.CustomerCode AND TT_Inventory.SupplierID = SupplierStock.SupplierID " +
                "AND TT_Inventory.ProductCode1 = SupplierStock.ProductCode1 AND TT_Inventory.WHSCode = SupplierStock.WHSCode " +

                p_oFilter.GetWhereString() +
                " Group By TT_PartMaster.CustomerCode, TT_PartMaster.SupplierID, TT_PartMaster.ProductCode1, TT_PartMaster.ProductCode2, " +
                "TT_PartMaster.ProductCode3, Description, SPQ, IsStandardPackaging, UOM, TT_PartMaster.IsCPart, TT_PartMaster.CPartSPQ, ExtSD.PickableQty, " +
                "EHPStock.EHPQty, SupplierStock.SupplierQty ";
            break;
*/
        }

        public async Task<IEnumerable<T>> GetLoadingEntryListFromOutbound<T>(string customerCode, string whsCode, bool isSAP, IEnumerable<string> outboundJobNos) where T : LoadingEntryListQueryResult, new()
        {
            var result = await (from kh in dbContext.EKanbanHeaders
                                join c in dbContext.Customers on kh.FactoryID equals c.Code
                                join ob in dbContext.Outbounds on kh.OutJobNo equals ob.JobNo
                                where kh.FactoryID == customerCode
                                     && !dbContext.LoadingDetails.Any(ld => ld.OrderNo == kh.OrderNo)
                                     && ob.Status != OutboundStatus.Cancelled
                                     && ob.TransType != OutboundType.Return
                                     && ob.WHSCode == whsCode
                                     && c.WHSCode == whsCode
                                     && outboundJobNos.Contains(ob.JobNo)
                                     && ((isSAP && (kh.Status == (byte)EKanbanStatus.Imported || kh.Status == (byte)EKanbanStatus.InTransit)) ||
                                        (!isSAP && kh.Status == (int)EKanbanStatus.Imported && ob.Status != OutboundStatus.Completed))

                                select new T
                                {
                                    OrderNo = kh.OrderNo,
                                    OutboundJobNo = kh.OutJobNo,
                                    ETD = ob.ETD,
                                    OutboundStatus = ob.Status,
                                    Remark = ob.Remark,
                                    TransportNo = ob.TransportNo
                                }).ToListAsync();

            return await GetLoadingEntryListSupplier(result, customerCode);
        }

        public async Task<IEnumerable<T>> GetLoadingEntryList<T>(string customerCode, string whsCode, bool isSAP, IEnumerable<string> orderNos) where T : LoadingEntryListQueryResult, new()
        {
            var result = await (from kh in dbContext.EKanbanHeaders
                                join c in dbContext.Customers on kh.FactoryID equals c.Code
                                join obs in dbContext.Outbounds on kh.OutJobNo equals obs.JobNo into outbounds
                                from ob in outbounds.DefaultIfEmpty()
                                where kh.FactoryID == customerCode
                                     && !dbContext.LoadingDetails.Any(ld => ld.OrderNo == kh.OrderNo)
                                     && ob.Status != OutboundStatus.Cancelled
                                     && ob.TransType != OutboundType.Return
                                     && ob.TransType != OutboundType.ScannerManualEntry
                                     && ob.WHSCode == whsCode
                                     && c.WHSCode == whsCode
                                     && (orderNos == null || orderNos.Contains(kh.OrderNo))
                                     && ((isSAP && (kh.Status == (byte)EKanbanStatus.Imported || kh.Status == (byte)EKanbanStatus.InTransit)) ||
                                        (!isSAP && kh.Status == (int)EKanbanStatus.Imported && ob.Status != OutboundStatus.Completed))

                                select new T
                                {
                                    OrderNo = kh.OrderNo,
                                    OutboundJobNo = kh.OutJobNo,
                                    ETD = ob.ETD,
                                    OutboundStatus = ob.Status,
                                    Remark = ob.Remark,
                                    TransportNo = ob.TransportNo
                                }).ToListAsync();

            return await GetLoadingEntryListSupplier(result, customerCode);
        }

        private async Task<IEnumerable<T>> GetLoadingEntryListSupplier<T>(IEnumerable<T> result, string customerCode) where T : LoadingEntryListQueryResult, new()
        {
            foreach (var row in result)
            {
                var supplier = await (from khd in dbContext.EKanbanHeaders
                                      join ou in dbContext.Outbounds on new { JobNo = khd.OutJobNo, RefNo = khd.OrderNo }
                                          equals new { ou.JobNo, ou.RefNo }
                                      join od in dbContext.OutboundDetails on ou.JobNo equals od.JobNo
                                      join sms in dbContext.SupplierMasters on new { khd.FactoryID, od.SupplierID }
                                            equals new { sms.FactoryID, sms.SupplierID } into supmas
                                      from sm in supmas.DefaultIfEmpty()
                                      orderby od.SupplierID
                                      where ou.RefNo == row.OrderNo && khd.FactoryID == customerCode
                                      select new { od.SupplierID, sm.CompanyName }).FirstOrDefaultAsync();
                if (supplier != null)
                {
                    row.SupplierID = supplier.SupplierID;
                    row.SupplierName = supplier.CompanyName;
                }
            }
            return result;
        }

        public EKanbanSummaryQueryResult GetSummaryQuantitiesFromEKanban(string refNo, string productCode, string supplierId)
        {
            var ekanbanDetailRows = (from eKanban in dbContext.EKanbanDetails
                                     where eKanban.OrderNo == refNo
                                           && eKanban.ProductCode == productCode
                                           && eKanban.SupplierID == supplierId
                                     select new
                                     {
                                         eKanban.OrderNo,
                                         eKanban.ProductCode,
                                         eKanban.SupplierID,
                                         eKanban.QuantityReceived,
                                         eKanban.QuantitySupplied
                                     }).ToList();

            return ekanbanDetailRows.GroupBy(g => new { g.OrderNo, g.ProductCode, g.SupplierID }).Select(g =>
                                             new EKanbanSummaryQueryResult
                                             {
                                                 OrderNo = g.Key.OrderNo,
                                                 ProductCode = g.Key.ProductCode,
                                                 SupplierId = g.Key.SupplierID,
                                                 TotalReceived = g.Sum(v => v.QuantityReceived),
                                                 TotalSupplied = g.Sum(v => v.QuantitySupplied)
                                             }).FirstOrDefault(); // TODO if the order no is different as there are 2 the same product codes on the list, what do we do????
        }

        public async Task<IEnumerable<OutboundDetailPickingQueryResult>> GetOutboundDetailPickingResultList(string jobNo)
        {
            var groupedResults = (await (from detail in dbContext.OutboundDetails
                                         join pl in dbContext.PickingLists on new { detail.JobNo, detail.ProductCode, detail.LineItem } equals new { pl.JobNo, pl.ProductCode, pl.LineItem }
                                         where detail.JobNo == jobNo
                                         orderby pl.SupplierID
                                         select new { detail, pl }).ToListAsync())
                                .GroupBy(g => new { g.pl.JobNo, g.pl.LineItem, g.pl.ProductCode })
                                .Select(r => new OutboundDetailPickingQueryResult
                                {
                                    OutboundDetail = r.Select(v => v.detail).First(),
                                    TotalPickedQty = r.Where(l => !String.IsNullOrEmpty(l.pl.PickedBy)).Sum(l => l.pl.Qty),
                                    TotalPickedPkg = r.Count(l => l.pl.Qty > 0 && !String.IsNullOrEmpty(l.pl.PickedBy))
                                }).ToList();

            return groupedResults;
        }
        public async Task<(double BalanceQty, long BalancePkg)?> GetInventoryLastTransactionBalance(string customerCode, string productCode)
        {
            var result = await dbContext.InvTransactions.Where(t => t.CustomerCode == customerCode && t.ProductCode == productCode)
                .OrderByDescending(t => t.SystemDateTime)
                .Select(t => new { t.BalanceQty, t.BalancePkg })
                .FirstOrDefaultAsync();
            if (result != null)
                return (result.BalanceQty, result.BalancePkg);
            return null;
        }
        public async Task<(double BalanceQty, long BalancePkg)?> GetInventoryLastTransactionPerWHSBalance(string customerCode, string productCode, string whsCode)
        {
            var result = await dbContext.InvTransactionsPerWHS.Where(t => t.CustomerCode == customerCode
                        && t.ProductCode == productCode
                        && t.WHSCode == whsCode)
                .OrderByDescending(t => t.SystemDateTime)
                .Select(t => new { t.BalanceQty, t.BalancePkg })
                .FirstOrDefaultAsync();
            if (result != null)
                return (result.BalanceQty, result.BalancePkg);
            return null;
        }
        public async Task<decimal?> GetInventoryLastTransactionPerSupplierBalance(string customerCode, string productCode, string supplierId, Ownership ownership)
        {
            return await dbContext.InvTransactionsPerSupplier.Where(t => t.CustomerCode == customerCode
                && t.ProductCode == productCode
                && t.SupplierID == supplierId
                && t.Ownership == ownership)
                .OrderByDescending(t => t.SystemDateTime)
                .Select(t => t.BalanceQty)
                .FirstOrDefaultAsync();
        }
        public async Task<IList<StorageDetailExtendedQueryResult>> GetStorageDetailListEuro(StorageDetailExtendedQueryFilter filter)
        {
            var query = GetStorageDetailWithLocationListQuery(filter);
            var result = await query.Select(q => new StorageDetailExtendedQueryResult()
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

            }).ToListAsync();

            return result;
        }
        public async Task<IList<string>> GetStoragePIDsForEKanban(StorageDetailExtendedQueryFilter filter)
        {
            var query = dbContext.StorageDetails.Select(s => s);
            if (filter.Statuses?.Any() != null)
            {
                query = query.Where(q => filter.Statuses.Contains(q.Status));
            }
            if (filter.OutJobNo != null)
            {
                query = query.Where(q => q.OutJobNo == filter.OutJobNo);
            }
            return await query.Select(q => q.PID).ToListAsync();
        }

        public async Task<IEnumerable<PickingList>> GetPickingLists(IEnumerable<string> jobNos, IEnumerable<int> lineItems = null)
        {
            var query = dbContext.PickingLists.AsQueryable();
            if (jobNos.Count() == 1)
                query = query.Where(pl => pl.JobNo == jobNos.First());
            else
                query = query.Where(pl => jobNos.Contains(pl.JobNo));

            if (lineItems?.Any() == true)
            {
                if (lineItems.Count() == 1)
                    query = query.Where(pl => pl.LineItem == lineItems.First());
                else
                    query = query.Where(pl => lineItems.Contains(pl.LineItem));
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<PickingListForCargoOutCheckingQueryResult>> GetPickingListForCargoOutChecking(string jobNo)
        {
            return await (from pl in dbContext.PickingLists
                          join st in dbContext.StorageDetails on pl.PID equals st.PID into sds
                          from sd in sds.DefaultIfEmpty()
                          join ib in dbContext.Inbounds on sd.InJobNo equals ib.JobNo into ibs
                          from inbound in ibs.DefaultIfEmpty()
                          where pl.JobNo == jobNo
                          orderby pl.LineItem
                          select new PickingListForCargoOutCheckingQueryResult()
                          {
                              PickingList = pl,
                              StockQty = sd != null ? sd.Qty : new decimal?(),
                              StockAllocatedQty = sd != null ? sd.AllocatedQty : new decimal?(),
                              OutboundJobNo = sd != null ? sd.OutJobNo : string.Empty,
                              StockStatus = sd != null ? sd.Status : new StorageStatus?(),
                              InboundStatus = inbound != null ? (byte)inbound.Status : new byte?(),
                              InboundJobNo = inbound != null ? inbound.JobNo : string.Empty,
                              StockWhsCode = sd != null ? sd.WHSCode : string.Empty,
                              StockPID = sd != null ? sd.PID : string.Empty,
                              CustomerCode = sd != null ? sd.CustomerCode : string.Empty
                          }).ToListAsync();
        }
        public async Task<IEnumerable<T>> GetOutboundDetailList<T>(string jobNo) where T : OutboundDetailQueryResult, new()
        {
            var query = from detail in dbContext.OutboundDetails
                        join outbound in dbContext.Outbounds on detail.JobNo equals outbound.JobNo
                        join partMaster in dbContext.PartMasters
                            on new { detail.ProductCode, detail.SupplierID, outbound.CustomerCode }
                            equals new { ProductCode = partMaster.ProductCode1, partMaster.SupplierID, partMaster.CustomerCode }
                        join uom in dbContext.UOMs on partMaster.UOM equals uom.Code
                        join ud in dbContext.UOMDecimals
                            on new { partMaster.CustomerCode, partMaster.UOM, Status = (byte)1 }
                            equals new { ud.CustomerCode, ud.UOM, ud.Status } into uds
                        from uomdec in uds.DefaultIfEmpty()
                        where detail.JobNo == jobNo
                        orderby detail.LineItem
                        select new T()
                        {
                            JobNo = detail.JobNo,
                            LineItem = detail.LineItem,
                            ProductCode = detail.ProductCode,
                            SupplierID = detail.SupplierID,
                            Qty = detail.Qty,
                            PickedQty = detail.PickedQty,
                            Pkg = detail.Pkg,
                            PickedPkg = detail.PickedPkg,
                            Status = detail.Status,
                            DecimalNum = uomdec != null ? uomdec.DecimalNum : 0,
                            TotalReceived = 0,
                            TotalSupplied = 0,
                            UOM = uom.Name,
                            IsCPart = partMaster.IsCPart == 1,
                            CPartSPQ = partMaster.CPartSPQ
                        };

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetStockTransferDetailList<T>(string jobNo) where T : StockTransferDetailQueryResult, new()
        {
            // also GetDetailListWithProductCodeName
            var query = from detail in dbContext.StockTransferDetails
                        join st in dbContext.StockTransfers on detail.JobNo equals st.JobNo
                        join storage in dbContext.StorageDetails on detail.PID equals storage.PID
                        join partMaster in dbContext.PartMasters on new { storage.ProductCode, storage.SupplierID, storage.CustomerCode }
                            equals new { ProductCode = partMaster.ProductCode1, partMaster.SupplierID, partMaster.CustomerCode }
                        join udec in dbContext.UOMDecimals on new { partMaster.CustomerCode, partMaster.UOM, Status = (byte)1 }
                            equals new { udec.CustomerCode, udec.UOM, udec.Status } into uomd
                        from ud in uomd.DefaultIfEmpty()

                        join inbound in dbContext.Inbounds on storage.InJobNo equals inbound.JobNo

                        join pm in dbContext.PriceMasters
                            on new { partMaster.CustomerCode, partMaster.SupplierID, partMaster.ProductCode1 }
                            equals new { pm.CustomerCode, pm.SupplierID, pm.ProductCode1 } into priceM
                        from priceMaster in priceM.DefaultIfEmpty()

                        where detail.JobNo == jobNo
                        orderby detail.LineItem
                        select new T()
                        {
                            JobNo = detail.JobNo,
                            LineItem = detail.LineItem,
                            PID = detail.PID,
                            OriginalWHSCode = detail.OriginalWHSCode,
                            OriginalSupplierID = detail.OriginalSupplierID,
                            OriginalLocationCode = detail.OriginalLocationCode,
                            LocationCode = detail.LocationCode,
                            TransferredBy = detail.TransferredBy,
                            TransferredDate = detail.TransferredDate,
                            WHSCode = detail.WHSCode,
                            SupplierID = partMaster.SupplierID,
                            ProductCode1 = partMaster.ProductCode1,
                            Description = partMaster.Description,
                            Qty = storage.Qty,
                            Price = priceMaster != null ? priceMaster.SellingPrice : 0,
                            DecimalNum = ud != null ? ud.DecimalNum : 0,
                            InboundDate = storage.InboundDate,
                            DaysInStock = DateTime.Now.Date.Subtract(storage.InboundDate.Date).TotalDays,
                            PIDValue = (storage.Qty == 0 ? storage.OriginalQty : storage.Qty) *
                                (storage.SellingPrice == 0 ? (storage.BuyingPrice ?? 0) : (storage.SellingPrice ?? 0)),
                            Currency = inbound.Currency ?? ""
                        };

            return await query.ToListAsync();

        }

        public async Task<IEnumerable<T>> GetStockTransferSummaryList<T>(string orderNo) where T : StockTransferSummaryQueryResult, new()
        {
            var query = from esh in dbContext.EStockTransferHeaders
                        join esd in dbContext.EStockTransferDetails on esh.OrderNo equals esd.OrderNo

                        join std in (from st in dbContext.StockTransfers
                                     join std in dbContext.StockTransferDetails on st.JobNo equals std.JobNo
                                     join storageDetail in dbContext.StorageDetails on std.PID equals storageDetail.PID
                                     where st.RefNo != ""
                                     group new { st.RefNo, storageDetail.ProductCode, storageDetail.SupplierID, PickedQty = storageDetail.Qty, st.JobNo, storageDetail.PID }
                                     by new { st.RefNo, storageDetail.ProductCode, storageDetail.SupplierID, st.JobNo }
                                     into g
                                     select new
                                     {
                                         g.Key.RefNo,
                                         g.Key.ProductCode,
                                         g.Key.SupplierID,
                                         g.Key.JobNo,
                                         PickedQty = g.Sum(i => i.PickedQty),
                                         PickedPkg = g.Count()
                                     }) on new { esd.OrderNo, esd.ProductCode, esh.StockTransferJobNo } equals new { OrderNo = std.RefNo, std.ProductCode, StockTransferJobNo = std.JobNo } into stds
                        from st in stds.DefaultIfEmpty()

                        where esh.OrderNo == orderNo
                        group new { esh.OrderNo, esd.ProductCode, esd.SupplierID, esd.Quantity, PickedQty = (decimal?)st.PickedQty, PickedPkg = (int?)st.PickedPkg }
                        by new { esh.OrderNo, esd.ProductCode, esd.SupplierID, PickedQty = (decimal?)st.PickedQty, PickedPkg = (int?)st.PickedPkg }
                        into gr
                        select new T
                        {
                            OrderNo = gr.Key.OrderNo,
                            ProductCode = gr.Key.ProductCode,
                            SupplierID = gr.Key.SupplierID,
                            Quantity = gr.Sum(i => i.Quantity),
                            Pkg = gr.Count(),
                            PickedQty = gr.Key.PickedQty.HasValue ? gr.Key.PickedQty.Value : 0,
                            PickedPkg = gr.Key.PickedPkg.HasValue ? gr.Key.PickedPkg.Value : 0
                        };

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetLoadingDetailList<T>(string jobNo) where T : LoadingDetailQueryResult, new()
        {
            var result = await (from ld in dbContext.LoadingDetails
                                join loading in dbContext.Loadings on ld.JobNo equals loading.JobNo
                                join kh in dbContext.EKanbanHeaders on ld.OrderNo equals kh.OrderNo
                                join ob in dbContext.Outbounds on kh.OutJobNo equals ob.JobNo
                                where ld.JobNo == jobNo
                                orderby ld.OrderNo
                                select new
                                {
                                    loading.CustomerCode,
                                    loading.JobNo,
                                    ld.OrderNo,
                                    ld.AddedBy,
                                    ld.AddedDate,
                                    ld.ETD,
                                    ld.OutJobNo,
                                    ld.SupplierID,
                                    ob.XDock,
                                    OutboundStatus = ob.Status,
                                    ob.CommInvNo,
                                    ob.NoOfPallet
                                }).ToListAsync(); ;

            List<T> data = new List<T>();
            foreach (var row in result)
            {
                var item = new T
                {
                    JobNo = row.JobNo,
                    OrderNo = row.OrderNo,
                    AddedBy = row.AddedBy,
                    AddedDate = row.AddedDate,
                    ETD = row.ETD,
                    OutJobNo = row.OutJobNo,
                    SupplierID = row.SupplierID,
                    OutboundStatus = row.OutboundStatus,
                    CommInvNo = row.CommInvNo,
                    CompanyName = "EHP",
                    XDock = row.XDock
                };
                var detailRows = await (from khs in dbContext.EKanbanHeaders
                                        join ob in dbContext.Outbounds on new { JobNo = khs.OutJobNo, RefNo = khs.OrderNo } equals new { ob.JobNo, ob.RefNo }
                                        join od in dbContext.OutboundDetails on ob.JobNo equals od.JobNo
                                        join pl in dbContext.PickingLists on new { ob.JobNo, od.LineItem } equals new { pl.JobNo, pl.LineItem }
                                        join sds in dbContext.StorageDetails on pl.PID equals sds.PID into sdetails
                                        from sd in sdetails.DefaultIfEmpty()
                                        join ibs in dbContext.Inbounds on sd.InJobNo equals ibs.JobNo into inbounds
                                        from ib in inbounds.DefaultIfEmpty()
                                        where khs.OrderNo == row.OrderNo
                                        select new { khs.OrderNo, od.SupplierID, ib.Currency, Ownership = sd != null ? sd.Ownership : new Ownership?() })
                                     .ToListAsync();
                if (detailRows.Any())
                {
                    var details = detailRows.GroupBy(g => g.OrderNo).Select(g => new
                    {
                        OrderNo = g.Key,
                        SupplierID = g.Min(i => i.SupplierID),
                        MixedCurrencies = g.Select(s => s.Currency).Distinct().Count() > 1,
                        Currency = g.Select(i => i.Currency).FirstOrDefault(),
                        CalculatedNoOfPallet = g.Count(),
                        NoOfPalletEHP = g.All(i => i.Ownership.HasValue) ? g.Where(i => i.Ownership == Ownership.EHP).Count() : new int?(),
                        NoOfPalletSupplier = g.All(i => i.Ownership.HasValue) ? g.Where(i => i.Ownership == (byte)Ownership.Supplier).Count() : new int?(),
                    }).FirstOrDefault();

                    var sms = dbContext.SupplierMasters.Where(sm => sm.FactoryID == row.CustomerCode
                            && sm.SupplierID == details.SupplierID).FirstOrDefault();

                    item.CompanyName = sms?.CompanyName ?? "EHP";
                    item.KDSupplierID = details.SupplierID;
                    item.Currency = details.MixedCurrencies ? null : details.Currency;
                    item.MixedCurrencies = details.MixedCurrencies;
                    item.NoOfPallet = details.CalculatedNoOfPallet;
                    item.NoOfPalletsEHP = details.NoOfPalletEHP;
                    item.NoOfPalletsSupplier = details.NoOfPalletSupplier;
                }
                data.Add(item);
            }
            return data;
        }

        public async Task<IEnumerable<T>> GetInboundDetailListWithPrice<T>(string jobNo, int? lineItem = null) where T : InboundDetailQueryResult, new()
        {
            var data = await (
                from inboundDetail in dbContext.InboundDetails
                join inbound in dbContext.Inbounds on inboundDetail.JobNo equals inbound.JobNo
                join pm in dbContext.PartMasters on new { inbound.CustomerCode, inbound.SupplierID, inboundDetail.ProductCode }
                    equals new { pm.CustomerCode, pm.SupplierID, ProductCode = pm.ProductCode1 }
                join uom in dbContext.UOMs on pm.UOM equals uom.Code
                join priceM in dbContext.PriceMasters on new { pm.CustomerCode, pm.SupplierID, pm.ProductCode1 }
                    equals new { priceM.CustomerCode, priceM.SupplierID, priceM.ProductCode1 } into prm
                from priceMaster in prm.DefaultIfEmpty()
                join uomD in dbContext.UOMDecimals on new { pm.CustomerCode, pm.UOM, Status = (byte)1 }
                    equals new { uomD.CustomerCode, uomD.UOM, uomD.Status } into uomDs
                from uomDec in uomDs.DefaultIfEmpty()
                where inbound.JobNo == jobNo
                    && (!lineItem.HasValue || inboundDetail.LineItem == lineItem.Value)
                orderby inboundDetail.LineItem
                select new T
                {
                    JobNo = inboundDetail.JobNo,
                    LineItem = inboundDetail.LineItem,
                    ProductCode = inboundDetail.ProductCode,
                    Qty = inboundDetail.Qty,
                    NoOfPackage = inboundDetail.NoOfPackage,
                    NoOfLabel = inboundDetail.NoOfLabel,
                    ControlCode1 = inboundDetail.ControlCode1,
                    ControlCode2 = inboundDetail.ControlCode2,
                    ControlCode3 = inboundDetail.ControlCode3,
                    ControlCode4 = inboundDetail.ControlCode4,
                    ControlCode5 = inboundDetail.ControlCode5,
                    ControlCode6 = inboundDetail.ControlCode6,
                    Remark = inboundDetail.Remark,
                    CustomerCode = inbound.CustomerCode,
                    WHSCode = inbound.WHSCode,
                    UOM = pm.UOM,
                    UOMName = uom.Name,
                    DecimalNum = uomDec != null ? uomDec.DecimalNum : 0,
                    IsDefected = pm.IsDefected == 1,
                    Currency = string.IsNullOrEmpty(inbound.Currency) ? (priceMaster != null ? priceMaster.Currency : "") : inbound.Currency,
                    BuyingPrice = (priceMaster != null ? priceMaster.BuyingPrice : 0),
                    LineValue = 0,
                    ResidualValue = 0,
                    PkgNo = inboundDetail.NoOfPackage,
                    Width = inboundDetail.Width,
                    Height = inboundDetail.Height,
                    Length = inboundDetail.Length
                }).ToListAsync();
            var excludeStatuses = new StorageStatus[] { StorageStatus.Cancelled, StorageStatus.Splitted, StorageStatus.Decant };
            foreach (var row in data)
            {
                var storageLineValues = await GetStorageDetailInboundPriceValues(row.JobNo, row.LineItem);
                row.LineValue = storageLineValues.LineValue;
                row.ResidualValue = storageLineValues.ResidualValue;
            }
            return data;
        }

        public async Task<IEnumerable<T>> GetStoragePutawayList<T>(string inJobNo, int? lineItem) where T : StorageDetailQueryResult, new()
        {
            return await (from sd in dbContext.StorageDetails
                          join su in dbContext.Users on sd.PutawayBy equals su.Code into sus
                          from systemUser in sus.DefaultIfEmpty()
                          join pm in dbContext.PartMasters on new { sd.CustomerCode, sd.SupplierID, sd.ProductCode }
                              equals new { pm.CustomerCode, pm.SupplierID, ProductCode = pm.ProductCode1 }
                              //join uomD in dbContext.UOMDecimals on new { pm.CustomerCode, pm.UOM, Status = (byte)1 }
                              //    equals new { uomD.CustomerCode, uomD.UOM, uomD.Status } into uomDs
                              //from uomDec in uomDs.DefaultIfEmpty()
                          join exP in dbContext.ExternalPIDs on sd.PID equals exP.PID into exps
                          from externalPID in exps.DefaultIfEmpty()
                          where sd.InJobNo == inJobNo
                              && sd.Status != StorageStatus.Cancelled && sd.Status != StorageStatus.ZeroOut
                              && (!lineItem.HasValue || sd.LineItem == lineItem.Value)
                          orderby sd.LineItem ascending, sd.SeqNo
                          select new T
                          {
                              InJobNo = inJobNo,
                              LineItem = sd.LineItem,
                              SeqNo = sd.SeqNo,
                              ProductCode = sd.ProductCode,
                              PID = sd.PID,
                              ExternalPID = externalPID.ExternalID,
                              ExternalSystem = externalPID.ExternalSystem,
                              PutawayBy = sd.PutawayBy,
                              PutawayByName = systemUser != null ? $"{systemUser.FirstName}, {systemUser.LastName}" : sd.PutawayBy,
                              PutawayDate = sd.PutawayDate,
                              LocationCode = sd.LocationCode,
                              Qty = sd.Qty,
                              QtyPerPkg = sd.QtyPerPkg,
                              GroupID = sd.GroupID
                          }).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetPartMasterListBySupplier<T>(string customerCode, string supplierID) where T : PartMasterBySupplierQueryResult, new()
        {
            return await (from pm in dbContext.PartMasters
                          join uom in dbContext.UOMs on pm.UOM equals uom.Code
                          join packageType in dbContext.PackageTypes on pm.PackageType equals packageType.Code
                          join uomD in dbContext.UOMDecimals on new { pm.CustomerCode, pm.UOM, Status = (byte)1 }
                              equals new { uomD.CustomerCode, uomD.UOM, uomD.Status } into uomDs
                          from uomDec in uomDs.DefaultIfEmpty()
                          where pm.CustomerCode == customerCode && pm.SupplierID == supplierID && pm.Status == (int)ValueStatus.Active
                          select new T
                          {
                              SupplierID = pm.SupplierID,
                              CustomerCode = pm.CustomerCode,
                              ProductCode1 = pm.ProductCode1,
                              Description = pm.Description,
                              IsDefected = pm.IsDefected,
                              IsStandardPackaging = pm.IsStandardPackaging,
                              Height = pm.HeightTT,
                              Width = pm.WidthTT,
                              Length = pm.LengthTT,
                              GrossWeight = pm.GrossWeightTT,
                              NetWeight = pm.NetWeightTT,
                              PackageType = pm.PackageType,
                              SPQ = pm.SPQ,
                              Status = pm.Status,
                              UOM = pm.UOM,
                              UOMName = uom.Name,
                              PkgTypeName = packageType.Name,
                              DecimalNum = uomDec != null ? uomDec.DecimalNum : 0
                          }).ToListAsync();
        }

        public async Task<IEnumerable<OutboundStatus>> GetOutboundStatusesForLoading(string jobNo)
        {
            var statuses = (await (from ob in dbContext.Outbounds
                                   join ek in dbContext.EKanbanHeaders on ob.JobNo equals ek.OutJobNo
                                   join ld in dbContext.LoadingDetails on ek.OrderNo equals ld.OrderNo
                                   where ld.JobNo == jobNo
                                   select (int?)ob.Status
                 ).ToListAsync()).Where(s => s.HasValue)
                 .Select(s => Enum.Parse<OutboundStatus>(s.Value.ToString()));
            return statuses;
        }

        public async Task<IEnumerable<string>> GetCycleCountJobNos(IEnumerable<string> productCodes, string whsCode)
        {
            var ccStatuses = new CycleCountStatus[] { CycleCountStatus.New, CycleCountStatus.Download, CycleCountStatus.Counting, CycleCountStatus.Outstanding };
            return (await (from ccd in dbContext.CycleCountDetails
                           join cc in dbContext.CycleCounts on ccd.JobNo equals cc.JobNo
                           join customer in dbContext.Customers on cc.CustomerCode equals customer.Code
                           orderby cc.JobNo
                           where productCodes.Contains(ccd.ProductCode)
                               && ccStatuses.Contains(cc.Status)
                               && cc.WHSCode == whsCode
                           select cc.JobNo).ToListAsync()).Distinct();
        }

        public async Task<string> GetCurrencyForLoading(string jobNo)
        {
            var detailRows = await (from ld in dbContext.LoadingDetails
                                    join khs in dbContext.EKanbanHeaders on ld.OrderNo equals khs.OrderNo
                                    join ob in dbContext.Outbounds on new { JobNo = khs.OutJobNo, RefNo = khs.OrderNo } equals new { ob.JobNo, ob.RefNo }
                                    join od in dbContext.OutboundDetails on ob.JobNo equals od.JobNo
                                    join pl in dbContext.PickingLists on new { ob.JobNo, od.LineItem } equals new { pl.JobNo, pl.LineItem }
                                    join sd in dbContext.StorageDetails on pl.PID equals sd.PID
                                    join ib in dbContext.Inbounds on sd.InJobNo equals ib.JobNo
                                    where ld.JobNo == jobNo
                                    select new { khs.OrderNo, ib.Currency })
                     .ToListAsync();

            if (!detailRows.Any()) return null;
            var details = detailRows.GroupBy(g => g.OrderNo).Select(g => new
            {
                OrderNo = g.Key,
                MixedCurrencies = g.Select(s => s.Currency).Distinct().Count() > 1,
                Currency = g.Select(i => i.Currency).FirstOrDefault()
            }).Select(c => c.Currency);

            return details.Distinct().Count() > 1 ? "" : details.FirstOrDefault();
        }


        public async Task<int> GetNoOfPalletsForLoading(string jobNo)
        {
            return await (from ob in dbContext.Outbounds
                          join ld in dbContext.LoadingDetails on ob.JobNo equals ld.OutJobNo
                          join pl in dbContext.PickingLists on ob.JobNo equals pl.JobNo
                          where ld.JobNo == jobNo
                          select new { pl.JobNo, pl.LineItem }).CountAsync();
        }
        public async Task<int> GetNoOfPalletsForOutbound(string jobNo)
        {
            return await (from ob in dbContext.Outbounds
                          join pl in dbContext.PickingLists on ob.JobNo equals pl.JobNo
                          where ob.JobNo == jobNo
                          select new { pl.JobNo, pl.LineItem }).CountAsync();
        }

        public async Task<IEnumerable<OutboundBillingDetailQueryResult>> GetOutboundBillingDetail(string orderNo)
        {
            var result = await (
                from header in dbContext.EKanbanHeaders
                join detail in dbContext.EKanbanDetails on header.OrderNo equals detail.OrderNo
                where header.OrderNo == orderNo && !string.IsNullOrEmpty(detail.SupplierID)
                select new
                {
                    header.OutJobNo,
                    header.FactoryID,
                    header.OrderNo,
                    detail.ProductCode,
                    detail.SupplierID,
                    detail.QuantitySupplied,
                    detail.BillingNo,
                    QtyPerPkg = dbContext.StorageDetails.Where(s =>
                        s.OutJobNo == header.OutJobNo &&
                        s.CustomerCode == header.FactoryID &&
                        s.SupplierID == detail.SupplierID &&
                        s.ProductCode == detail.ProductCode && 
                        s.Ownership == Ownership.EHP)
                    .Sum(g => g.QtyPerPkg)
                }).ToListAsync();
           
            return result
                .GroupBy(g => new
                {
                    g.OutJobNo,
                    g.FactoryID,
                    g.OrderNo,
                    g.ProductCode,
                    g.SupplierID,
                    g.BillingNo,
                    g.QtyPerPkg
                }).Select(g => new OutboundBillingDetailQueryResult()
                {
                    OutJobNo = g.Key.OutJobNo,
                    FactoryID = g.Key.FactoryID,
                    OrderNo = g.Key.OrderNo,
                    ProductCode = g.Key.ProductCode,
                    SupplierID = g.Key.SupplierID,
                    BillingNo = g.Key.BillingNo,
                    Qty = g.Sum(v => v.QuantitySupplied) - g.Key.QtyPerPkg
                }).ToList();


            /*                    case EEKANBANDetailGetListMethod.GetBillingDetail:
    m_strSQL =
        "SELECT EKANBANHeader.OutJobNo AS JobNo, EKANBANHeader.FactoryID AS CustomerCode, EKANBANHeader.OrderNo AS RefNo, " +
        "       EKANBANDetail.ProductCode, EKANBANDetail.SupplierID, SUM(EKANBANDetail.QuantitySupplied) - ISNULL(ES.EQty,0) AS Qty, " +
        "       EKANBANDetail.BillingNo " +
        "FROM EKANBANHeader " +
        "     INNER JOIN EKANBANDetail " +
        "        ON EKANBANHeader.OrderNo = EKANBANDetail.OrderNo " +
        "LEFT JOIN " +
        "    (SELECT OUTJOBNO, CUSTOMERCODE, SUPPLIERID, PRODUCTCODE, SUM(QTYPERPKG) as EQty " +
        "     FROM TT_StorageDetail WITH (NOLOCK) " +
        "     WHERE Ownership = 1 " +
        "     GROUP BY OUTJOBNO, CUSTOMERCODE, SUPPLIERID, PRODUCTCODE) ES " +
        "ON EKANBANHeader.OutJobNo = ES.OutJobNo " +
        "AND EKANBANHeader.FactoryID = ES.CustomerCode " +
        "AND EKANBANDetail.SupplierID = ES.SupplierID " +
        "AND EKANBANDetail.ProductCode = ES.ProductCode " +
        p_oFilter.GetWhereString() +
        " GROUP BY EKANBANHeader.OutJobNo, EKANBANHeader.FactoryID, EKANBANHeader.OrderNo, " +
        "         EKANBANDetail.ProductCode, EKANBANDetail.SupplierID, " +
        "         EKANBANDetail.BillingNo, ES.EQty ";
    break;
*/
        }
        public async Task<IList<PickingListDataToDownloadQueryResult>> GetPickingListDataToDownload(PickingListToDownloadQueryFilter queryFilter)
        {
            var query = from pl in dbContext.PickingLists
                        join od in dbContext.OutboundDetails on new { pl.JobNo, pl.ProductCode, pl.SupplierID } equals new { od.JobNo, od.ProductCode, od.SupplierID }
                        join o in dbContext.Outbounds on pl.JobNo equals o.JobNo
                        join pm in dbContext.PartMasters on new { pl.ProductCode, o.CustomerCode, pl.SupplierID }
                            equals new { ProductCode = pm.ProductCode1, pm.CustomerCode, pm.SupplierID }
                        join uom in dbContext.UOMs on pm.UOM equals uom.Code
                        join uomDec in dbContext.UOMDecimals on new { UOM = uom.Code, pm.CustomerCode }
                            equals new { uomDec.UOM, uomDec.CustomerCode }
                        join extPid in dbContext.ExternalPIDs on pl.PID equals extPid.PID into exP
                        from externalPID in exP.DefaultIfEmpty()
                        join cc in dbContext.ControlCodes on pl.ControlCode equals cc.Code into ccd
                        from controlCode in ccd.DefaultIfEmpty()
                        where queryFilter.JobNos.Contains(pl.JobNo) && string.IsNullOrEmpty(pl.PickedBy)
                        orderby pl.LocationCode, pl.ProductCode, pl.ControlDate, pl.InboundDate
                        select new PickingListDataToDownloadQueryResult()
                        {
                            JobNo = pl.JobNo,
                            PID = pl.PID,
                            ProductCode1 = pm.ProductCode1,
                            ExternalID = externalPID.ExternalID,
                            LocationCode = pl.LocationCode,
                            WHSCode = pl.WHSCode,
                            SupplierID = pl.SupplierID,
                            Version = pl.Version,
                            InboundDate = pl.InboundDate,
                            ProductionLine = pl.ProductionLine,
                            IsPalletItem = pm.IsPalletItem,
                            ControlCodeName = controlCode.Name,
                            ControlCodeType = pl.ControlCodeType,
                            ControlCodeValue = pl.ControlCodeValue,
                            ControlDate = pl.ControlDate,
                            IsStandardPackaging = pm.IsStandardPackaging,
                            SPQ = pm.SPQ,
                            Pkg = od.Pkg,
                            PickedPkg = od.PickedPkg,
                            DecimalNum = uomDec.DecimalNum,
                            Qty = od.Qty,
                            TotalPickQty = od.Qty - (dbContext.PickingLists.Where(p => !string.IsNullOrEmpty(p.PickedBy)
                                          && p.JobNo == od.JobNo && p.ProductCode == od.ProductCode).Sum(p => p.Qty))
                        };

            if (queryFilter.ProductionLineNumbers?.Any() == true)
            {
                query = query.Where(i => queryFilter.ProductionLineNumbers.Contains(i.ProductionLine));
            }
            if (queryFilter.IsPalletItem.HasValue)
            {
                query = query.Where(i => i.IsPalletItem == (queryFilter.IsPalletItem.Value ? 1 : 0));
            }
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<KanbanListGroupQueryResult>> GetKanbanListGroupByProductCode(string orderNo)
        {
            return await dbContext.EKanbanDetails.Where(e => e.OrderNo == orderNo)
                .GroupBy(e => new { e.OrderNo, e.ProductCode, e.SupplierID })
                .Select(g => new KanbanListGroupQueryResult
                {
                    OrderNo = g.Key.OrderNo,
                    ProductCode = g.Key.ProductCode,
                    SupplierID = g.Key.SupplierID,
                    SumQtySupplied = g.Sum(i => i.QuantitySupplied),
                    SumQtyReceived = g.Sum(i => i.QuantityReceived),
                    NoOfKanban = g.Count()
                })
                .ToListAsync();
        }

        public async Task<IList<EKanbanDetail>> GetEKanbanDetailForPicking(EKanbanForPickingQueryFilter filter)
        {
            var query = dbContext.EKanbanDetails.AsQueryable();
            if (filter.OrderNo != null)
            {
                query = query.Where(q => q.OrderNo == filter.OrderNo);
            }
            if (filter.ProductCode != null)
            {
                query = query.Where(q => q.ProductCode == filter.ProductCode);
            }
            if (filter.SupplierIdEmpty.HasValue)
            {
                query = query.Where(q => filter.SupplierIdEmpty.Value ? String.IsNullOrEmpty(q.SupplierID) : !String.IsNullOrEmpty(q.SupplierID));
            }
            return await query.OrderBy(detail => detail.ProductCode).ThenBy(detail => detail.SupplierID).ToListAsync();
        }

        public async Task<IEnumerable<EStockTransferDetailForFiltersQueryResult>> GetEStockTransferDetailForFilters(string jobNo)
        {
            return await (from st in dbContext.StockTransfers
                          join estd in dbContext.EStockTransferDetails on st.RefNo equals estd.OrderNo
                          where st.JobNo == jobNo
                          select new EStockTransferDetailForFiltersQueryResult { SupplierID = estd.SupplierID, ProductCode = estd.ProductCode })
                              .ToListAsync();
        }

        public async Task<IEnumerable<StorageDetailWithPartInfoQueryResult>> GetStorageDetailWithPartInfo(StorageDetailQueryFilter filter)
        {
            var query = from sd in dbContext.StorageDetails
                        join pm in dbContext.PartMasters on new { sd.CustomerCode, sd.SupplierID, sd.ProductCode }
                                    equals new { pm.CustomerCode, pm.SupplierID, ProductCode = pm.ProductCode1 }
                        join location in dbContext.Locations.DefaultIfEmpty() on new { sd.LocationCode, sd.WHSCode }
                                    equals new { LocationCode = location.Code, location.WHSCode }
                        join inbound in dbContext.Inbounds.DefaultIfEmpty() on sd.InJobNo equals inbound.JobNo
                        join externalPID in dbContext.ExternalPIDs.DefaultIfEmpty() on sd.PID equals externalPID.PID into ext
                        from externalP in ext.DefaultIfEmpty()
                        join uomDec in dbContext.UOMDecimals.DefaultIfEmpty() on new { pm.CustomerCode, pm.UOM }
                                equals new { uomDec.CustomerCode, uomDec.UOM } into uomD
                        from uomdecimal in uomD.Where(d => d.Status == 1).DefaultIfEmpty()

                        where (filter.CustomerCode == null || pm.CustomerCode == filter.CustomerCode)
                            && (filter.ProductCode == null || pm.ProductCode1 == filter.ProductCode)
                            && (filter.SupplierId == null || pm.SupplierID == filter.SupplierId)
                            && (filter.WHSCode == null || sd.WHSCode == filter.WHSCode)
                            && (filter.GroupID == null || sd.GroupID == filter.GroupID)
                            && !String.IsNullOrEmpty(sd.LocationCode)
                            && String.IsNullOrEmpty(sd.OutJobNo)
                            && (location.Type == LocationType.Normal || location.Type == LocationType.ExtSystem)

                        orderby sd.Ownership descending, sd.InboundDate ascending, // FIFO
                            location.IsPriority descending,
                            (sd.PutawayDate.HasValue ? sd.PutawayDate.Value.Date : new DateTime?()) ascending,
                            sd.InJobNo ascending, sd.Version, sd.LocationCode, sd.PID

                        select new StorageDetailWithPartInfoQueryResult()
                        {
                            StorageDetail = sd,
                            DecimalNum = uomdecimal != null ? uomdecimal.DecimalNum : 0,
                            Location = location,
                            ExternalPID = externalP != null ? externalP.ExternalID : null,
                            RefNo = inbound.RefNo,
                            SPQ = pm.SPQ
                            //CommInvNo = null // TODO add joins if needed at some point
                        };

            if (filter.Ownership.HasValue)
            {
                query = query.Where(q => q.StorageDetail.Ownership == filter.Ownership.Value);
            }
            if (filter.QtyGreaterThan.HasValue)
            {
                query = query.Where(q => q.StorageDetail.Qty > filter.QtyGreaterThan.Value
                        && q.StorageDetail.Qty - q.StorageDetail.AllocatedQty > filter.QtyGreaterThan.Value);
            }
            if (filter.Statuses?.Any() == true)
            {
                query = query.Where(q => filter.Statuses.Contains(q.StorageDetail.Status));
            }
            if (!string.IsNullOrWhiteSpace(filter.InJobNo))
            {
                query = query.Where(q => q.StorageDetail.InJobNo == filter.InJobNo);
            }
            if (filter.LocationType.HasValue)
            {
                query = query.Where(q => q.Location.Type == filter.LocationType.Value);
            }
            if (filter.PID != null)
            {
                query = query.Where(q => q.StorageDetail.PID == filter.PID);
            }
            //todo: no iLog
            var ilogInboundCatId = GetILogLocationCategory(ILogLocationCategory.InboundLocation);
            query = query.Where(q => q.Location.ILogLocationCategoryId != ilogInboundCatId);
            return await query.ToListAsync();

            /*                        m_strSQL = "SELECT TT_StorageDetail.*, ISNULL(TT_UOMDecimal.DecimalNum,0) AS DecimalNum,
             *                        TT_Location.*, " +
           "       TT_ExternalPID.ExternalPID, TT_Inbound.RefNo, SPQ, ISNULL(STF.CommInvNo,'') as CommInvNo " + 
			"FROM   TT_StorageDetail " + (...)
           "       LEFT OUTER JOIN (SELECT CommInvNo, PID From TT_StockTransfer INNER JOIN TT_StockTransferDetail " +
           "       ON TT_StockTransfer.JobNo = TT_StockTransferDetail.JobNo " +
           "       WHERE TT_StockTransfer.Status = " + (int)EStockTransferStatus.Completed + 
           "       AND TT_StockTransfer.JobNo Not In (Select STFJobNo From TT_STFReversalMaster WHERE Status = " + (int)EStockTransferReversalStatus.Completed + ")) STF " +
           "       ON STF.PID = TT_StorageDetail.PID " +
			p_oFilter.GetFilterState();*/
        }

        public async Task<IEnumerable<SFTStorageDetailWithPartInfoQueryResult>> GetSFTStorageDetailWithPartInfo(SFTStorageDetailQueryFilter filter)
        {
            var query = from sd in dbContext.StorageDetails
                        join pm in dbContext.PartMasters on new { sd.CustomerCode, sd.SupplierID, sd.ProductCode }
                                    equals new { pm.CustomerCode, pm.SupplierID, ProductCode = pm.ProductCode1 }
                        join location in dbContext.Locations.DefaultIfEmpty() on new { sd.LocationCode, sd.WHSCode }
                                    equals new { LocationCode = location.Code, location.WHSCode }
                        join uomDec in dbContext.UOMDecimals on new { pm.CustomerCode, pm.UOM }
                                equals new { uomDec.CustomerCode, uomDec.UOM } into uomD
                        from uomdecimal in uomD.Where(d => d.Status == 1).DefaultIfEmpty()

                        where pm.CustomerCode == filter.CustomerCode
                            && sd.WHSCode == filter.WHSCode
                            && sd.Qty > 0
                            && sd.Ownership == Ownership.Supplier
                            && sd.Status == StorageStatus.Putaway

                        orderby sd.InboundDate ascending

                        select new SFTStorageDetailWithPartInfoQueryResult()
                        {
                            StorageDetail = sd,
                            Location = location,
                            Description = pm.Description,
                            DecimalNum = uomdecimal != null ? uomdecimal.DecimalNum : 0,
                            UOM = uomdecimal != null ? uomdecimal.UOM : null,
                            DaysInStock = DateTime.Now.Date.Subtract(sd.InboundDate.Date).TotalDays
                        };

            if (!string.IsNullOrWhiteSpace(filter.InJobNo))
            {
                query = query.Where(q => q.StorageDetail.InJobNo == filter.InJobNo);
            }
            if (filter.ProductCodes?.Any() == true)
            {
                query = query.Where(q => filter.ProductCodes.Contains(q.StorageDetail.ProductCode));
            }
            if (filter.SupplierIds?.Any() == true)
            {
                query = query.Where(q => filter.SupplierIds.Contains(q.StorageDetail.SupplierID));
            }
            //todo: no ilog
            var ilogInboundCatId = GetILogLocationCategory(ILogLocationCategory.InboundLocation);
            query = query.Where(q => q.Location.ILogLocationCategoryId != ilogInboundCatId);
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetDistinctStorageSupplierList<T>(string customerCode, string whsCode) where T : SupplierQueryResult, new()
        {
            // GetDistinctStorageSupplier
            IQueryable<T> query = from sd in dbContext.StorageDetails
                join sm in dbContext.SupplierMasters on new { FactoryID = sd.CustomerCode, sd.SupplierID } equals new { sm.FactoryID, sm.SupplierID }
                where sd.CustomerCode == customerCode
                && sd.WHSCode == whsCode
                && sd.Status == StorageStatus.Putaway
                && sd.Ownership == Ownership.Supplier
                && sd.Qty > 0
                orderby sd.SupplierID ascending
                select new T
                {
                    SupplierID = sd.SupplierID,
                    CompanyName = sm.CompanyName
                };
            return await query.Distinct().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetDistinctStorageInJobNoList<T>(string customerCode, string supplierId, string whsCode) where T : InJobNoQueryResult, new()
        {
            return await dbContext.StorageDetails.Where(s =>
                        s.WHSCode == whsCode &&
                        s.CustomerCode == customerCode &&
                        s.SupplierID == supplierId &&
                        s.Status == StorageStatus.Putaway &&
                        s.Ownership == Ownership.Supplier &&
                        s.Qty > 0
                ).OrderBy(s => s.InJobNo)
                .Select(s => new T
                {
                    JobNo = s.InJobNo,
                    InboundDate = s.InboundDate
                }).Distinct().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetCountries<T>() where T : CountryListQueryResult, new()
        {
            return await (from c in dbContext.Countries orderby c.Name select new T() { Code = c.Code, Name = c.Name }).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetPackageTypes<T>() where T : PackageTypeListQueryResult, new()
        {
            return await (from pt in dbContext.PackageTypes orderby pt.Name select new T() { Code = pt.Code, Name = pt.Name }).ToListAsync();
        }
        public async Task<IEnumerable<StorageDetail>> GetStorageDetailForLocationType(LocationType locationType, string inJobNo)
        {
            //ETT_StorageDetailGetListMethod.GetStorageDetailLocationTypeList:
            return await (from sd in dbContext.StorageDetails
                          join location in dbContext.Locations on new { Code = sd.LocationCode, sd.WHSCode }
                                equals new { location.Code, location.WHSCode }
                          where location.Type == locationType
                            && sd.InJobNo == inJobNo
                          select sd).ToListAsync();
        }

        public async Task<IEnumerable<ASNDetailWithSPQQueryResult>> GetASNDetailWithSPQList(string asnNo)
        {
            return await (from detail in dbContext.ASNDetails
                          join header in dbContext.ASNHeaders on detail.ASNNo equals header.ASNNo
                          join pms in dbContext.PartMasters on new { CustomerCode = header.FactoryID, header.SupplierID, detail.ProductCode }
                              equals new { pms.CustomerCode, pms.SupplierID, ProductCode = pms.ProductCode1 } into partms
                          from pm in partms.DefaultIfEmpty()
                          where detail.ASNNo == asnNo
                          orderby detail.LineItem
                          select new ASNDetailWithSPQQueryResult
                          {
                              ASNDetail = detail,
                              ProductCode = pm.ProductCode1,
                              PackageType = pm.PackageType,
                              Length = pm.LengthTT,
                              Width = pm.WidthTT,
                              Height = pm.HeightTT,
                              GrossWeight = pm.GrossWeightTT,
                              NetWeight = pm.NetWeightTT,
                              SPQ = pm.SPQ
                          }).ToListAsync();
        }

        public async Task<IEnumerable<InboundQtyByProductCodeQueryResult>> GetInboundDetailGroupByProductCode(string jobNo)
        {
            return await dbContext.InboundDetails.Where(i => i.JobNo == jobNo)
                .Select(i => new { i.ProductCode, i.Qty, i.NoOfPackage })
                .GroupBy(i => i.ProductCode)
                .Select(g => new InboundQtyByProductCodeQueryResult
                {
                    ProductCode = g.Key,
                    TotalQty = g.Sum(i => i.Qty),
                    TotalPkg = g.Sum(i => i.NoOfPackage)
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<T>> GetEKanbanDetailDistinctProductCodeList<T>(string orderNo, string productCode, string supplierId) where T : EKanbanDetailDistinctProductCodeQueryResult, new()
        {
            var query = dbContext.EKanbanDetails.AsQueryable();
            if (orderNo != null)
                query = query.Where(e => e.OrderNo == orderNo);
            if (productCode != null)
                query = query.Where(e => e.ProductCode == productCode);
            if (supplierId != null)
                query = query.Where(e => e.SupplierID == supplierId);

            return await query.GroupBy(e => new { e.OrderNo, e.ProductCode, e.SupplierID })
                .Select(e => new T()
                {
                    OrderNo = e.Key.OrderNo,
                    ProductCode = e.Key.ProductCode,
                    SupplierId = e.Key.SupplierID,
                    SumQty = e.Sum(i => i.Quantity),
                    SumQtySupplied = e.Sum(i => i.QuantitySupplied),
                    SumQtyReceived = e.Sum(i => i.QuantityReceived),
                    NoOfKanban = e.Count()
                }).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetEStockTransferDistinctProductCodeList<T>(string orderNo, string productCode, string supplierId) where T : EKanbanDetailDistinctProductCodeQueryResult, new()
        {
            var query = dbContext.EStockTransferDetails.AsQueryable();
            if (orderNo != null)
                query = query.Where(e => e.OrderNo == orderNo);
            if (productCode != null)
                query = query.Where(e => e.ProductCode == productCode);
            if (supplierId != null)
                query = query.Where(e => e.SupplierID == supplierId);

            return await query.GroupBy(e => new { e.OrderNo, e.ProductCode, e.SupplierID })
                .Select(e => new T()
                {
                    OrderNo = e.Key.OrderNo,
                    ProductCode = e.Key.ProductCode,
                    SupplierId = e.Key.SupplierID,
                    SumQty = e.Sum(i => i.Quantity),
                    SumQtySupplied = e.Sum(i => i.QuantitySupplied),
                    SumQtyReceived = e.Sum(i => i.QuantityReceived),
                    NoOfKanban = e.Count()
                }).ToListAsync();
        }

        public async Task<IEnumerable<StorageDetail>> GetExpiredStorageDetails(string whsCode, string supplierId, string factoryId, IEnumerable<String> productCodes)
        {
            var expiredStorageDetail = from s in dbContext.StorageDetails
                                       join expSunset in dbContext.SunsetExpiredAlerts on new { s.CustomerCode, s.SupplierID, s.ProductCode }
                                            equals new { CustomerCode = expSunset.FactoryID, expSunset.SupplierID, expSunset.ProductCode }
                                       where
                                           s.Status == StorageStatus.Putaway &&
                                           s.WHSCode == whsCode &&
                                           s.SupplierID == supplierId &&
                                           s.CustomerCode == factoryId &&
                                           productCodes.Contains(s.ProductCode) &&
                                           s.Ownership == Ownership.Supplier &&
                                           expSunset.SunsetPeriod != -1 &&
                                           s.PutawayDate <= DateTime.Now.Date.AddDays(expSunset.SunsetPeriod * -1)
                                       select s;
            return await expiredStorageDetail.ToListAsync();
        }

        public async Task<IEnumerable<AllocatedStorageDetailSummaryQueryResult>> GetStorageDetailSummaryListForGroup(string groupID)
        {
            return await dbContext.StorageDetails.AsQueryable().Where(s => s.GroupID == groupID)
                .GroupBy(q => new { q.CustomerCode, q.SupplierID, q.ProductCode, q.Ownership, q.WHSCode, q.Qty })
                .Select(q => new AllocatedStorageDetailSummaryQueryResult()
                {
                    CustomerCode = q.Key.CustomerCode,
                    SupplierID = q.Key.SupplierID,
                    ProductCode = q.Key.ProductCode,
                    Ownership = q.Key.Ownership,
                    WHSCode = q.Key.WHSCode,
                    AllocatedQty = q.Sum(i => i.AllocatedQty),
                    Qty = q.Sum(i => i.Qty),
                    AllocatedPkg = q.Count()
                }).ToListAsync();
        }

        public async Task<IEnumerable<AllocatedStorageDetailSummaryQueryResult>> GetAllocatedStorageDetailSummaryList(AllocatedStorageDetailSummaryQueryFilter filter)
        {
            if (string.IsNullOrWhiteSpace(filter.ProductCode)
                || string.IsNullOrWhiteSpace(filter.SupplierID)
                || string.IsNullOrWhiteSpace(filter.CustomerCode)
                || string.IsNullOrWhiteSpace(filter.WHSCode))
                return Enumerable.Empty<AllocatedStorageDetailSummaryQueryResult>();

            var statuses = new StorageStatus[] { StorageStatus.Allocated, StorageStatus.Picked, StorageStatus.Packed };
            var returnLocation = Enum.GetName(typeof(ExtSystemLocation), ExtSystemLocation.RETURN);

            var query = dbContext.StorageDetails.AsQueryable()
            .Where(q => q.ProductCode == filter.ProductCode && q.SupplierID == filter.SupplierID &&
                        q.CustomerCode == filter.CustomerCode && q.WHSCode == filter.WHSCode &&
                        statuses.Contains(q.Status) &&
                        q.LocationCode != returnLocation);

            return await query.GroupBy(q => new { q.CustomerCode, q.SupplierID, q.ProductCode, q.Ownership, q.WHSCode })
                .Select(q => new AllocatedStorageDetailSummaryQueryResult()
                {
                    CustomerCode = q.Key.CustomerCode,
                    SupplierID = q.Key.SupplierID,
                    ProductCode = q.Key.ProductCode,
                    Ownership = q.Key.Ownership,
                    WHSCode = q.Key.WHSCode,
                    AllocatedQty = q.Sum(i => i.AllocatedQty),
                    Qty = q.Sum(i => i.Qty),
                    AllocatedPkg = q.Count()
                }).ToListAsync();
        }

        public async Task<bool> HasAnyEStockTransferDiscrepancy(string jobNo)
        {
            var stockTransferLinkedToCallOff = 
                from esd in dbContext.EStockTransferDetails
                join std in
                    (from st in dbContext.StockTransfers
                        join stdet in dbContext.StockTransferDetails on st.JobNo equals stdet.JobNo
                        join storage in dbContext.StorageDetails on stdet.PID equals storage.PID
                        where st.JobNo == jobNo
                        group new { st.JobNo, st.RefNo, storage.ProductCode, storage.SupplierID, storage.Qty, storage.PID }
                        by new { st.JobNo, st.RefNo, storage.ProductCode, storage.SupplierID } into g
                        select new
                        {
                            g.Key.JobNo,
                            g.Key.RefNo,
                            g.Key.ProductCode,
                            g.Key.SupplierID,
                            PickedQty = g.Sum(i => i.Qty)
                        }) on new { esd.OrderNo, esd.ProductCode } equals new { OrderNo = std.RefNo, std.ProductCode }
                group new { esd.OrderNo, esd.ProductCode, esd.SupplierID, std.PickedQty, esd.Quantity }
                    by new { esd.OrderNo, esd.ProductCode, esd.SupplierID, std.PickedQty } into gr
                select new
                {
                    gr.Key.OrderNo,
                    gr.Key.ProductCode,
                    gr.Key.SupplierID,
                    gr.Key.PickedQty,
                    Quantity = gr.Sum(i => i.Quantity),
                };
            
            return await stockTransferLinkedToCallOff
                .Where(s => s.Quantity != s.PickedQty)
                .AnyAsync();
        }

        public async Task<decimal?> GetOutboundDetailUnallocatedQty(string customerCode, string supplierId, string productCode, string whsCode)
        {
            var query = from outbound in dbContext.Outbounds
                        join od in dbContext.OutboundDetails on outbound.JobNo equals od.JobNo
                        join pls in dbContext.PickingLists on new { od.JobNo, od.LineItem } equals new { pls.JobNo, pls.LineItem } into plss
                        from pl in plss.DefaultIfEmpty()
                        where outbound.CustomerCode == customerCode && od.SupplierID == supplierId 
                            && od.ProductCode == productCode && outbound.WHSCode == whsCode
                            && outbound.Status <= OutboundStatus.Packed
                        group new { outbound.CustomerCode, od.SupplierID, od.ProductCode, OutboundQty = od.Qty, PickingQty = pl.Qty }
                        by new { outbound.CustomerCode, od.SupplierID, od.ProductCode, od.Qty }
                        into g
                        select g.Key.Qty - g.Sum(l => l.PickingQty);
            
            return await query.FirstOrDefaultAsync();
        }

        private IQueryable<StorageDetailQueryable> GetStorageDetailWithLocationListQuery(StorageDetailExtendedQueryFilter filter)
        {
            var query = from storageDetail in dbContext.StorageDetails
                        join location in dbContext.Locations on new { Code = storageDetail.LocationCode, storageDetail.WHSCode }
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

        public async Task<StorageDetail> GetStorageDetailForFilter(StorageQueryFilter filter)
        {
            var data = await (from sd in dbContext.StorageDetails
                              where (filter.ProductCode == null || sd.ProductCode == filter.ProductCode)
                                  && (!filter.Qty.HasValue || sd.Qty == filter.Qty)
                                  && (!filter.Status.HasValue || sd.Status == filter.Status)
                                  && (filter.OutJobNo == null || sd.OutJobNo == filter.OutJobNo)
                              orderby sd.PID
                              select sd).ToListAsync();

            if (data.Any() && !String.IsNullOrEmpty(filter.LocationCode))
            {
                return data.FirstOrDefault(s => s.LocationCode == filter.LocationCode) ?? data.LastOrDefault();
            }
            else
            {
                return data.LastOrDefault();
            }
        }

        public async Task<string> GetLastPIDCode(string match)
        {
            return await dbContext.PIDCodes.Where(c => c.PIDNo.StartsWith(match))
                .OrderByDescending(c => c.PIDNo)
                .Select(c => c.PIDNo)
                .FirstOrDefaultAsync();
        }

        public async Task<string> GetLastGroupPIDCode(string match)
        {
            return await dbContext.StorageDetailGroups.Where(c => c.GroupID.StartsWith(match))
                .OrderByDescending(c => c.GroupID)
                .Select(c => c.GroupID)
                .FirstOrDefaultAsync();
        }

        public async Task<string> GetLastEKanbanOrderNo(string match)
        {
            return await dbContext.EKanbanHeaders.Where(e => e.OrderNo.StartsWith(match))
                .OrderByDescending(e => e.OrderNo).Select(e => e.OrderNo).FirstOrDefaultAsync();
        }

        public async Task<string> GetLastEStockTransferOrderNo(string match)
        {
            return await dbContext.EStockTransferHeaders.Where(e => e.OrderNo.StartsWith(match))
                .OrderByDescending(e => e.OrderNo).Select(e => e.OrderNo).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetOutboundDetailWithReceivedQtyList<T>(string jobNo) where T : OutboundDetailQueryResult, new()
        {
            var query = from detail in dbContext.OutboundDetails
                        join outbound in dbContext.Outbounds on detail.JobNo equals outbound.JobNo
                        join partMaster in dbContext.PartMasters
                            on new { detail.ProductCode, detail.SupplierID, outbound.CustomerCode }
                            equals new { ProductCode = partMaster.ProductCode1, partMaster.SupplierID, partMaster.CustomerCode }
                        join uom in dbContext.UOMs on partMaster.UOM equals uom.Code
                        //join inventory in repository.Inventory()
                        //    on new { detail.ProductCode, outbound.CustomerCode, outbound.WHSCode }
                        //    equals new { ProductCode = inventory.ProductCode1, inventory.CustomerCode, inventory.WHSCode }
                        join ud in dbContext.UOMDecimals
                            on new { partMaster.CustomerCode, partMaster.UOM, Status = (byte)1 }
                            equals new { ud.CustomerCode, ud.UOM, ud.Status } into uds
                        from uomdec in uds.DefaultIfEmpty()

                        where detail.JobNo == jobNo
                        orderby detail.LineItem
                        select new
                        {
                            dto = new T()
                            {
                                JobNo = detail.JobNo,
                                LineItem = detail.LineItem,
                                ProductCode = detail.ProductCode,
                                SupplierID = detail.SupplierID,
                                Qty = detail.Qty,
                                PickedQty = detail.PickedQty,
                                Pkg = detail.Pkg,
                                PickedPkg = detail.PickedPkg,
                                Status = detail.Status,
                                DecimalNum = uomdec != null ? uomdec.DecimalNum : 0,
                                TotalReceived = 0,
                                TotalSupplied = 0,
                                UOM = uom.Name
                            },
                            outbound.RefNo

                        };
            var result = await query.ToListAsync();

            var data = result.Select(q =>
            {
                var summaryData = GetSummaryQuantitiesFromEKanban(q.RefNo, q.dto.ProductCode, q.dto.SupplierID);
                q.dto.TotalReceived = summaryData.TotalReceived;
                q.dto.TotalSupplied = summaryData.TotalSupplied;
                return q.dto;
            });

            return data;
            /*					
             "SELECT *, ISNULL(TT_UOMDecimal.DecimalNum,0) AS DecimalNum  " + 
							"FROM   TT_OutboundDetail " + 
							"       INNER JOIN TT_Outbound ON TT_OutboundDetail.JobNo = TT_Outbound.JobNo " +
							"       INNER JOIN TT_PartMaster ON TT_OutboundDetail.ProductCode = TT_PartMaster.ProductCode1 " + 
							"       AND TT_PartMaster.SupplierID = TT_OutboundDetail.SupplierID AND TT_PartMaster.CustomerCode = TT_Outbound.CustomerCode " +  
							"       INNER JOIN TT_Inventory ON TT_OutboundDetail.ProductCode = TT_Inventory.ProductCode1 " + 
							"       AND TT_Inventory.CustomerCode = TT_Outbound.CustomerCode AND TT_Inventory.WHSCode = TT_Outbound.WHSCode " +         
							"       AND TT_Inventory.SupplierID = TT_OutboundDetail.SupplierID INNER JOIN TT_UOM ON TT_PartMaster.UOM = TT_UOM.Code" + 
							"       LEFT OUTER JOIN TT_UOMDecimal ON " + 
							"       TT_UOMDecimal.CustomerCode = TT_PartMaster.CustomerCode AND " + 
							"       TT_UOMDecimal.UOM = TT_PartMaster.UOM AND TT_UOMDecimal.Status = 1 Inner Join" + 
							" ( SELECT OrderNo, ProductCode as eProductCode, SupplierID as eSupplierID, Sum(QuantityReceived) as TotalReceived, Sum(QuantitySupplied) TotalSupplied From " +
							" EkanbanDetail Group by OrderNo, ProductCode, SupplierID ) eKanban on TT_Outbound.RefNo = eKanban.OrderNo and " +
							" TT_OutboundDetail.ProductCode = eKanban.eProductCode and  TT_OutboundDetail.SupplierID = eKanban.eSupplierID " +
							p_oFilter.GetFilterState();
            */
        }

        public string GetSuppliers(string jobNo)
        {
            return String.Join(", ", (from sm in dbContext.SupplierMasters
                                      join detail in dbContext.OutboundDetails on sm.SupplierID equals detail.SupplierID
                                      where detail.JobNo == jobNo
                                      select sm.CompanyName).Distinct().ToList());
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

        public async Task<IEnumerable<WHSTransferSummaryQueryResult>> GetWHSTransferSummaryList(string jobNo)
        {
            var query = from whsTransfer in dbContext.WHSTransferDetails
                        join sd in dbContext.StorageDetails on whsTransfer.PID equals sd.PID
                        where whsTransfer.JobNo == jobNo
                        select new
                        {
                            sd.ProductCode,
                            sd.Qty,
                            sd.QtyPerPkg
                        };
            return (await query.ToListAsync()).GroupBy(g => g.ProductCode)
                .Select(g => new WHSTransferSummaryQueryResult()
                {
                    ProductCode = g.Key,
                    TotalQty = g.Sum(v => v.Qty),
                    TotalPkg = Convert.ToInt32(g.Sum(v => Math.Ceiling(v.Qty / (v.QtyPerPkg > 0 ? v.QtyPerPkg : v.Qty))))

                });
        }

        public async Task<IEnumerable<string>> GetBondedStockJobNosWithoutCommInv(string jobNo)
        {
            // TODO we could check the commInv in the InvoiceRequests - should move the whole method to the new api
            return await (from l in dbContext.Loadings
                          join ld in dbContext.LoadingDetails on l.JobNo equals ld.JobNo
                          join ek in dbContext.EKanbanHeaders on ld.OrderNo equals ek.OrderNo
                          join ob in dbContext.Outbounds on ek.OutJobNo equals ob.JobNo
                          join sd in dbContext.StorageDetails on ob.JobNo equals sd.OutJobNo
                          where l.JobNo == jobNo
                            && string.IsNullOrWhiteSpace(ob.CommInvNo)
                            && sd.BondedStatus == (int)BondedStatus.Bonded
                          select ob.JobNo)
                          .Distinct()
                          .ToListAsync();
        }

        public async Task<IEnumerable<OutboundStatus>> GetDistinctLoadingOutboundList(string jobNo)
        {
            return (await
                (from ob in dbContext.Outbounds
                 join ek in dbContext.EKanbanHeaders on ob.JobNo equals ek.OutJobNo
                 join ld in dbContext.LoadingDetails on ek.OrderNo equals ld.OrderNo
                 join l in dbContext.Loadings on ld.JobNo equals l.JobNo
                 orderby ob.Status descending
                 where l.JobNo == jobNo
                 select ob.Status)
                 .ToListAsync())
                 .Select(o => (OutboundStatus)o);
        }

        public async Task<IEnumerable<ReportPrintingLog>> GetLastReportPrintingLogs(string jobNo)
        {
            var allReports = await dbContext.ReportPrintingLogs
                .OrderByDescending(r => r.PrintedDate)
                .Where(r => r.JobNo == jobNo)
                .ToListAsync();
            return allReports.Any() ?
              allReports.GroupBy(g => g.ReportName).Select(g => g.FirstOrDefault()) :
              Enumerable.Empty<ReportPrintingLog>();
        }

        public async Task<IEnumerable<T>> GetInboundIDTList<T>(string jobNo) where T : InboundIDTListItemQueryResult, new()
        {
            //"SELECT AH.ASNNo, Convert(Date,AH.CreatedDate,102) as CreatedDate, " +
            //            "AH.FactoryID, ModeOfTransport, Convert(Date, StoreArrivalDate, 102) as StoreArrivalDate, " +
            //            "AH.SupplierID, CompanyName, TotalPackages, TotalWeight, SupplierInvoiceNumber, " +
            //            "ProductCode, Convert(Date,ManufacturedDate, 102) as ManufacturedDate, BatchNo, " +
            //            "QtyPerOuter, NoOfOuter, (QtyPerOuter * NoOfOuter) as TotalQty " +
            //            "FROM TT_Inbound INNER JOIN " +
            //            "ASNHeader AH " +
            //            "ON TT_Inbound.IRNo = AH.ASNNo " +
            //            "INNER JOIN ASNDetail AD " +
            //            "ON AH.ASNNo = AD.ASNNo " +
            //            "INNER JOIN SupplierMaster " +
            //            "ON AH.FactoryID = SupplierMaster.FactoryID " +
            //            "AND AH.SupplierID = SupplierMaster.SupplierID " +
            // where TT_Inbound.JobNo = jobNo
            var query = from inbound in dbContext.Inbounds
                        join ah in dbContext.ASNHeaders on inbound.IRNo equals ah.ASNNo
                        join ad in dbContext.ASNDetails on ah.ASNNo equals ad.ASNNo
                        join sm in dbContext.SupplierMasters on new { ah.FactoryID, ah.SupplierID } equals new { sm.FactoryID, sm.SupplierID }
                        where inbound.JobNo == jobNo
                        select new T
                        {
                            ASNNo = ah.ASNNo,
                            CreatedDate = ah.CreatedDate,
                            FactoryID = ah.FactoryID,
                            ModeOfTransport = ah.ModeOfTransport,
                            StoreArrivalDate = ad.StoreArrivalDate,
                            SupplierID = ah.SupplierID,
                            CompanyName = sm.CompanyName,
                            TotalPackages = ah.TotalPackages,
                            TotalWeight = ah.TotalWeight,
                            SupplierInvoiceNumber = ah.SupplierInvoiceNumber,
                            ProductCode = ad.ProductCode,
                            ManufacturedDate = ad.ManufacturedDate,
                            BatchNo = ad.BatchNo,
                            QtyPerOuter = ad.QtyPerOuter,
                            NoOfOuter = ad.NoOfOuter
                        };
            return await query.ToListAsync();
        }

        public async Task<T> GetInboundExtendedAsync<T>(string jobNo) where T : InboundWithExtendedDataQueryResult, new()
        {
            var query = from inbound in dbContext.Inbounds
                        join customer in dbContext.Customers on inbound.CustomerCode equals customer.Code
                        join suppl in dbContext.SupplierMasters on new { inbound.SupplierID, FactoryID = inbound.CustomerCode }
                            equals new { suppl.SupplierID, suppl.FactoryID } into sup
                        from supplier in sup.DefaultIfEmpty()
                        join cu in dbContext.Users on inbound.CreatedBy equals cu.Code into cus
                        from users in cus.DefaultIfEmpty()
                        where inbound.JobNo == jobNo
                        select new T
                        {
                            JobNo = jobNo,
                            CustomerCode = inbound.CustomerCode,
                            CustomerName = customer != null ? customer.Name : null,
                            SupplierID = inbound.SupplierID,
                            SupplierName = supplier != null ? supplier.CompanyName : null,
                            Status = inbound.Status,
                            TransType = inbound.TransType,
                            Remark = inbound.Remark,
                            WHSCode = inbound.WHSCode,
                            IRNo = inbound.IRNo,
                            RefNo = inbound.RefNo,
                            ETA = inbound.ETA,
                            Charged = inbound.Charged,
                            CreatedBy = inbound.CreatedBy,
                            CreatedByName = users != null ? users.FullName : null,
                            CreatedDate = inbound.CreatedDate,
                            RevisedBy = inbound.RevisedBy,
                            RevisedDate = inbound.RevisedDate,
                            CancelledBy = inbound.CancelledBy,
                            CancelledDate = inbound.CancelledDate,
                            PutawayBy = inbound.PutawayBy,
                            PutawayDate = inbound.PutawayDate,
                            Currency = inbound.Currency,
                            IM4No = inbound.IM4No,
                            ContainerNo = null,
                            TotalResidualValue = 0,
                            TotalASNValue = 0,
                            CustomsDeclarationDate = inbound.CustomsDeclarationDate
                        };

            var entity = await query.FirstOrDefaultAsync();
            if (entity != null)
            {
                entity.ContainerNo = entity.TransType == InboundType.ASN && !string.IsNullOrEmpty(entity.IRNo) ? (await GetASNHeaderAsync(entity.IRNo))?.ContainerNoEU : null;
                var storageLineValues = await GetStorageDetailInboundPriceValues(entity.JobNo, null);
                entity.TotalASNValue = storageLineValues.LineValue;
                entity.TotalResidualValue = storageLineValues.ResidualValue;
            }
            return entity;
        }


        public async Task GetOutboundEDTDataAsync(string jobNo, Action<IDataReader> action)
        {
            var query = @"
        SELECT RIGHT(TT_Outbound.RefNo,10) as JobNo, TT_Outbound.CreatedDate as OutboundDate, 
        FactoryName, TT_Outbound.CustomerCode,
        TT_PickingList.SupplierID, UPPER(ISNULL(TT_Country.Code,'')) as CountryCode, 
        TT_PickingList.ProductCode, Description,
        IRNo,
        ISNULL(TT_UOM.Name,'') as UOM, CONVERT(decimal(10,3), 
        SUM(TT_PickingList.Qty)) as Qty, Count(PID) as PIDCount, 
        CASE WHEN (IsBonded = 0) THEN 'TT-E' ELSE 
        CASE WHEN (IsBonded = 1 AND UPPER(ISNULL(TT_Country.Code,'')) = 'IT') THEN 'TT-I' ELSE 'TT-N' END 
        END as InternalWHS 
        FROM TT_Outbound INNER JOIN TT_OutboundDetail 
        ON TT_Outbound.JobNo = TT_OutboundDetail.JobNo 
        INNER JOIN TT_PickingList 
        ON TT_OutboundDetail.JobNo = TT_PickingList.JobNo 
        AND TT_OutboundDetail.LineItem = TT_PickingList.LineItem 
        INNER JOIN ( 
           SELECT Distinct TT_StorageDetail.InboundDate, InJobNo, IRNo 
           FROM TT_Outbound INNER JOIN TT_PickingList 
           ON TT_Outbound.JobNo = TT_PickingList.JobNo 
           LEFT JOIN TT_StorageDetail 
           ON TT_PickingList.InboundDate = TT_StorageDetail.InboundDate 
           INNER JOIN TT_Inbound ON TT_StorageDetail.InJobNo = TT_Inbound.JobNo 
           WHERE TT_Outbound.JobNo = @jobNo
           and TT_StorageDetail.Status in (2,3,4,6)
        ) AllocatedASN 
        ON TT_PickingList.InboundDate = AllocatedASN.InboundDate 
        INNER JOIN SupplierMaster ON TT_PickingList.SupplierID = SupplierMaster.SupplierID 
        AND TT_Outbound.CustomerCode = SupplierMaster.FactoryID 
        INNER JOIN TT_PartMaster ON TT_Outbound.CustomerCode = TT_PartMaster.CustomerCode 
        AND TT_PickingList.SupplierID = TT_PartMaster.SupplierID AND TT_PickingList.ProductCode = TT_PartMaster.ProductCode1 
        INNER JOIN FactoryMaster ON TT_Outbound.CustomerCode = FactoryMaster.FactoryID 
        LEFT JOIN TT_UOM ON TT_PartMaster.UOM = TT_UOM.Code 
        LEFT JOIN TT_Country ON SupplierMaster.Country = TT_Country.Name 
        WHERE TT_Outbound.JobNo = @jobNo
        Group BY RIGHT(TT_Outbound.RefNo,10), TT_Outbound.CreatedDate, FactoryName, TT_Outbound.CustomerCode, 
        TT_PickingList.SupplierID, UPPER(ISNULL(TT_Country.Code,'')), TT_PickingList.ProductCode, Description, 
        IRNo, TT_UOM.Name, IsBonded 
        ORDER BY ProductCode";

            await ExecuteQueryAsync(query, action, new KeyValuePair<string, object>("@jobNo", jobNo));
        }

        public async Task GetStockTransferEDTDataAsync(string jobNo, Action<IDataReader> action)
        {
            var query = @"SELECT RIGHT(TT_StockTransfer.RefNo,10) as JobNo, TT_StockTransfer.CreatedDate as OutboundDate, 
                            FactoryName, TT_StockTransfer.CustomerCode, TT_StockTransferDetail.OriginalSupplierID AS SupplierID, UPPER(ISNULL(TT_Country.Code,'')) as CountryCode, 
                            TT_StorageDetail.ProductCode, Description, IRNo, ISNULL(TT_UOM.Name,'') as UOM, CONVERT(decimal(10,3), 
                            SUM(TT_StorageDetail.OriginalQty)) as Qty, Count(TT_StockTransferDetail.PID) as PIDCount,
                            CASE WHEN (IsBonded = 0) THEN 'TT-E' ELSE 
                               CASE WHEN (IsBonded = 1 AND UPPER(ISNULL(TT_Country.Code,'')) = 'IT') THEN 'TT-I' ELSE 'TT-N' END 
                            END as InternalWHS
                            FROM TT_StockTransfer INNER JOIN TT_StockTransferDetail 
                            ON TT_StockTransfer.JobNo = TT_StockTransferDetail.JobNo
                            INNER JOIN TT_StorageDetail
                            ON TT_StockTransferDetail.PID = TT_StorageDetail.PID
                            INNER JOIN TT_Inbound ON TT_StorageDetail.InJobNo = TT_Inbound.JobNo
                            INNER JOIN SupplierMaster ON TT_StorageDetail.SupplierID = SupplierMaster.SupplierID
                            AND TT_StorageDetail.CustomerCode = SupplierMaster.FactoryID 
                            INNER JOIN TT_PartMaster ON TT_StorageDetail.CustomerCode = TT_PartMaster.CustomerCode 
                            AND TT_StorageDetail.SupplierID = TT_PartMaster.SupplierID AND TT_StorageDetail.ProductCode = TT_PartMaster.ProductCode1 
                            INNER JOIN FactoryMaster ON TT_StockTransfer.CustomerCode = FactoryMaster.FactoryID
                            LEFT JOIN TT_UOM ON TT_PartMaster.UOM = TT_UOM.Code
                            LEFT JOIN TT_Country ON SupplierMaster.Country = TT_Country.Name
                            WHERE TT_StockTransfer.JobNo = @jobNo
                            Group BY RIGHT(TT_StockTransfer.RefNo,10), TT_StockTransfer.CreatedDate, FactoryName, TT_StockTransfer.CustomerCode,
                            TT_StockTransferDetail.OriginalSupplierID, UPPER(ISNULL(TT_Country.Code,'')), TT_StorageDetail.ProductCode, Description,
                            IRNo, TT_UOM.Name, IsBonded ORDER BY ProductCode";

            await ExecuteQueryAsync(query, action, new KeyValuePair<string, object>("@jobNo", jobNo));
        }

        public async Task<IEnumerable<string>> GetSystemModuleNamesForGroup(string groupCode)
        {
            return await (from ar in dbContext.AccessRights
                          join sm in dbContext.SystemModules on ar.ModuleCode equals sm.Code
                          where ar.Status == 1 && ar.GroupCode == groupCode
                          select sm.ModuleName).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAccessGroups<T>(AccessGroupFilter filter) where T : AccessGroupSimpleQueryResult, new()
        {
            var query = dbContext.AccessGroups.Select(g => new T
            {
                Code = g.Code,
                Description = g.Description,
                Status = g.Status
            });
            if (!string.IsNullOrEmpty(filter.Code))
            {
                query = query.Where(g => g.Code.ToLower().Contains(filter.Code.ToLower()));
            }
            if (!string.IsNullOrEmpty(filter.Description))
            {
                query = query.Where(g => g.Description.ToLower().Contains(filter.Description.ToLower()));
            }
            if (filter.Status.HasValue)
            {
                query = query.Where(g => g.Status == filter.Status);
            }

            switch (filter.OrderBy)
            {
                case "description":
                    query = filter.Desc ? query.OrderByDescending(g => g.Description) : query.OrderBy(g => g.Description);
                    break;
                case "status":
                    query = filter.Desc ? query.OrderByDescending(g => g.Status) : query.OrderBy(g => g.Status);
                    break;
                case "code":
                default:
                    query = filter.Desc ? query.OrderByDescending(g => g.Code) : query.OrderBy(g => g.Code);
                    break;
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<StockTransferDetailGroupListQueryResult>> GetStockTransferDetailGroupList(string jobNo)
        {
            // ETT_StockTransferDetailGetListMethod.GetDetailGroupList:
            return await (from std in dbContext.StockTransferDetails
                          join sd in dbContext.StorageDetails on std.PID equals sd.PID
                          where std.JobNo == jobNo
                          orderby sd.ProductCode ascending
                          orderby std.OriginalSupplierID ascending
                          group new { sd.CustomerCode, sd.SupplierID, sd.ProductCode, sd.Qty }
                          by new { sd.CustomerCode, sd.SupplierID, sd.ProductCode }
                        into g
                          select new StockTransferDetailGroupListQueryResult
                          {
                              CustomerCode = g.Key.CustomerCode,
                              SupplierID = g.Key.SupplierID,
                              ProductCode = g.Key.ProductCode,
                              TotalQty = g.Sum(i => i.Qty),
                              TotalPkg = g.Count()
                          }).ToListAsync();

        }

        public async Task<StockTransferTotalValueQueryResult> GetStockTransferTotalValueQueryResult(string jobNo)
        {
            var details = await GetStockTransferDetailList<StockTransferDetailQueryResult>(jobNo);
            if (details.Any())
            {
                return new StockTransferTotalValueQueryResult
                {
                    OutboundTotalValue = details.Sum(d => d.PIDValue),
                    Currency = details.Select(d => d.Currency).Distinct().Count() > 1 ? null : details.Select(d => d.Currency).First(),
                    MixedCurrency = details.Select(d => d.Currency).Distinct().Count() > 1
                };
            }
            return new StockTransferTotalValueQueryResult();
        }

        public async Task<bool> HasCancelledOrderLines(string orderNo)
        {
            var query =
                from ed in dbContext.EKanbanDetails
                where ed.OrderNo == orderNo
                group ed by new { ed.OrderNo, ed.SupplierID, ed.ProductCode } into g
                where g.Sum(e => e.Quantity) > 0 && g.Sum(e => e.QuantitySupplied) == 0
                select 1;
            return await query.AnyAsync();
        }

        public async Task<List<OutboundOrderSummaryQueryResult>> GetOutboundOrderSummary(string outJobNo)
        {
            var query =
                from o in dbContext.Outbounds
                where o.JobNo == outJobNo
                join ed in dbContext.EKanbanDetails on o.RefNo equals ed.OrderNo
                group ed by new { ed.OrderNo, ed.SupplierID, ed.ProductCode } into g
                select new OutboundOrderSummaryQueryResult()
                {
                    SupplierID = g.Key.SupplierID,
                    ProductCode = g.Key.ProductCode,
                    OrderQty = g.Sum(e => e.Quantity),
                    OutboundQty = g.Sum(e => e.QuantitySupplied)
                };
            return await query.ToListAsync();
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
        public async Task<PickingList> GetPickingListAsync(string jobNo, int lineItem, int seqNo)
        {
            return await dbContext.PickingLists.FindAsync(jobNo.Trim(), lineItem, seqNo);
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
        public async Task<Inbound> GetInboundAsync(string inJobNo)
        {
            return await dbContext.Inbounds.FindAsync(inJobNo.Trim());
        }
        public async Task<InboundDetail> GetInboundDetailAsync(string jobNo, int lineItem)
        {
            return await dbContext.InboundDetails.FindAsync(jobNo.Trim(), lineItem);
        }
        public async Task<StorageDetailGroup> GetStorageDetailGroupAsync(string groupID)
        {
            return await dbContext.StorageDetailGroups.FindAsync(groupID.Trim());
        }
        public async Task<StorageDetail> GetStorageDetailAsync(string pid)
        {
            return await dbContext.StorageDetails.FindAsync(pid.Trim());
        }
        public async Task<Inventory> GetInventoryAsync(string customerCode, string supplierID, string productCode, string wHSCode, Ownership ownership)
        {
            return await dbContext.Inventory.FindAsync(customerCode.Trim(), supplierID.Trim(), productCode.Trim(), wHSCode.Trim(), ownership);
        }
        public async Task<EKanbanDetail> GetEKanbanDetailAsync(string orderNo, string productCode, string serialNo)
        {
            return await dbContext.EKanbanDetails.FindAsync(orderNo.Trim(), productCode.Trim(), serialNo.Trim());
        }
        public async Task<IEnumerable<EKanbanHeader>> GetEKanbanHeadersAsync(params string[] orderNos)
        {
            return await dbContext.EKanbanHeaders.Where(e => orderNos.Contains(e.OrderNo.Trim())).ToListAsync();
        }
        public async Task<EKanbanHeader> GetEKanbanHeaderAsync(string orderNo)
        {
            return await dbContext.EKanbanHeaders.FindAsync(orderNo.Trim());
        }
        public async Task<EStockTransferHeader> GetEStockTransferHeaderAsync(string orderNo)
        {
            return await dbContext.EStockTransferHeaders.FindAsync(orderNo.Trim());
        }
        public async Task<EStockTransferDetail> GetEStockTransferDetailAsync(string orderNo, string productCode, string serialNo)
        {
            if (serialNo == null)
                return await dbContext.EStockTransferDetails.Where(s => s.OrderNo == orderNo && s.ProductCode == productCode)
                    .OrderBy(s => s.SerialNo).FirstOrDefaultAsync();

            return await dbContext.EStockTransferDetails.FindAsync(orderNo.Trim(), productCode, serialNo);
        }
        public async Task<OutboundQRCode> GetOutboundQRCodeAsync(string jobNo)
        {
            return await dbContext.OutboundQRCodes.FindAsync(jobNo.Trim());
        }
        public async Task<InventoryControl> GetInventoryControlAsync(string customerCode)
        {
            return await dbContext.InventoryControls.FindAsync(customerCode.Trim());
        }
        public async Task<ProductCode> GetProductCodeAsync(string code)
        {
            return await dbContext.ProductCodes.FindAsync(code.Trim());
        }
        public async Task<ControlCode> GetControlCodeAsync(string code)
        {
            return await dbContext.ControlCodes.FindAsync(code.Trim());
        }
        public async Task<SupplierMaster> GetSupplierMasterAsync(string factoryId, string supplierId)
        {
            return await dbContext.SupplierMasters.FindAsync(factoryId.Trim(), supplierId.Trim());
        }
        public async Task<IEnumerable<SupplierMaster>> GetSupplierMasterListAsync(string factoryId)
        {
            var query = from supplierMasters in dbContext.SupplierMasters
                        where supplierMasters.FactoryID == factoryId.Trim()
                        orderby supplierMasters.CompanyName, supplierMasters.SupplierID
                        select supplierMasters;
            return (await query.ToListAsync());
        }
        public async Task<PriceMaster> GetPriceMasterAsync(string customerCode, string supplierId, string productCode)
        {
            return await dbContext.PriceMasters.FindAsync(customerCode.Trim(), supplierId.Trim(), productCode.Trim());
        }
        public async Task<AccessLock> GetAccessLockAsync(string jobNo)
        {
            return await dbContext.AccessLocks.FindAsync(jobNo.Trim());
        }
        public async Task<PickingListAllocatedPID> GetPickingListAllocatedPIDAsync(string jobNo, int lineItem, int seqNo)
        {
            return await dbContext.PickingListAllocatedPIDs.FindAsync(jobNo, lineItem, seqNo);
        }

        public IQueryable<Outbound> GetLoadingOutboundQuery(string loadingJobNo)
        {
            return from ob in dbContext.Outbounds
                        join ek in dbContext.EKanbanHeaders on ob.JobNo equals ek.OutJobNo
                        join ld in dbContext.LoadingDetails on ek.OrderNo equals ld.OrderNo
                        where ld.JobNo == loadingJobNo
                        select ob;
        }

        public async Task<IEnumerable<Outbound>> GetLoadingOutboundList(string jobNo, IEnumerable<OutboundStatus> statuses, bool invertStatusFilter = false)
        {
            var query = GetLoadingOutboundQuery(jobNo);
            if (statuses?.Any() == true)
            {
                if (invertStatusFilter)
                    query = query.Where(ob => !statuses.Contains(ob.Status));
                else
                    query = query.Where(ob => statuses.Contains(ob.Status));
            }
            return await query.ToListAsync();
        }
        
        public async Task<Owner> GetOwnerAsync(string code)
        {
            return await dbContext.Owners.FindAsync(code);
        }
        public async Task<Loading> GetLoadingAsync(string jobNo)
        {
            return await dbContext.Loadings.FindAsync(jobNo);
        }

        public async Task<LoadingDetail> GetLoadingDetailAsync(string jobNo, string orderNo)
        {
            return await dbContext.LoadingDetails.FindAsync(jobNo, orderNo);
        }

        public async Task<JobCode> GetJobCodeAsync(int code)
        {
            return await dbContext.JobCodes.FindAsync(code);
        }

        public async Task<Location> GetLocationAsync(string code, string whsCode)
        {
            return await dbContext.Locations.FindAsync(code, whsCode);
        }

        public async Task<Customer> GetCustomerAsync(string code, string whsCode)
        {
            return await dbContext.Customers.FindAsync(code, whsCode);
        }

        public async Task<ASNHeader> GetASNHeaderAsync(string asnNo)
        {
            return await dbContext.ASNHeaders.FindAsync(asnNo);
        }
        public async Task<ASNDetail> GetASNDetailAsync(string asnNo, int lineItem)
        {
            return await dbContext.ASNDetails.FindAsync(asnNo, lineItem);
        }
        public async Task<SupplierItemMaster> GetSupplierItemMasterAsync(string factoryID, string supplierID, string productCode)
        {
            return await dbContext.SupplierItemMasters.FindAsync(factoryID, supplierID, productCode);
        }
        public async Task<SunsetExpiredAlert> GetSunsetExpiredAlertAsync(string factoryID, string supplierID, string productCode)
        {
            return await dbContext.SunsetExpiredAlerts.FindAsync(factoryID, supplierID, productCode);
        }
        public async Task<ItemMaster> GetItemMasterAsync(string factoryID, string supplierID, string productCode)
        {
            return await dbContext.ItemMasters.FindAsync(factoryID, supplierID, productCode);
        }
        public async Task<User> GetUserAsync(string userCode)
        {
            return await dbContext.Users.FindAsync(userCode);
        }
        public async Task<AccessGroup> GetAccessGroupAsync(string userCode)
        {
            return await dbContext.AccessGroups.FindAsync(userCode);
        }

        public async Task<StockTransfer> GetStockTransferAsync(string jobNo)
        {
            return await dbContext.StockTransfers.FindAsync(jobNo);

        }
        public async Task<StockTransferDetail> GetStockTransferDetailAsync(string jobNo, int lineItem)
        {
            return await dbContext.StockTransferDetails.FindAsync(jobNo, lineItem);
        }

        public async Task<IEnumerable<CompleteOutboundQueryResult>> GetCompleteOutboundData(IEnumerable<string> jobNos)
        {
            return await (from o in dbContext.Outbounds
                           join pls in dbContext.PickingLists on o.JobNo equals pls.JobNo into plls
                           from pl in plls.DefaultIfEmpty()
                           join sds in dbContext.StorageDetails on pl.PID equals sds.PID into sdds
                           from sd in sdds.DefaultIfEmpty()
                           join inbs in dbContext.Inbounds on sd.InJobNo equals inbs.JobNo into inbbs
                           from inb in inbbs.DefaultIfEmpty()
                           join invs in dbContext.Inventory on
                           new { sd.CustomerCode, pl.SupplierID, pl.ProductCode, o.WHSCode, sd.Ownership } equals
                           new { invs.CustomerCode, invs.SupplierID, ProductCode = invs.ProductCode1, invs.WHSCode, invs.Ownership } into invvs
                           from inv in invvs.DefaultIfEmpty()
                           where jobNos.Contains(o.JobNo)
                           select new CompleteOutboundQueryResult
                           {
                               Outbound = o,
                               PickingList = pl,
                               StorageDetail = sd,
                               Inbound = inb,
                               Inventory = inv
                           }).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetUOMListWithDecimal<T>(string customerCode) where T : UOMWithDecimalQueryResult, new()
        {
            return await (from uom in dbContext.UOMs
                          join uomDec in dbContext.UOMDecimals on uom.Code equals uomDec.UOM into ud
                          from subUomDec in ud.DefaultIfEmpty()
                          where subUomDec.CustomerCode == customerCode
                          orderby uom.Name
                          select new T()
                          {
                              Code = uom.Code,
                              Name = uom.Name,
                              DecimalNum = subUomDec != null ? subUomDec.DecimalNum : 0
                          }).ToListAsync();
        }

        public async Task<IEnumerable<UnloadingPoint>> GetUnloadingPoints(string customerCode)
        {
            return await dbContext.UnloadingPoint
                .Where(x => x.CustomerCode == customerCode)
                .ToListAsync();
        }

        public UnloadingPoint GetUnloadingPoint(int Id)
        {
            return dbContext.UnloadingPoint
                .Where(x => x.Id == Id)
                .SingleOrDefault();
        }

        public async Task<int?> GetDefaultUnloadingPointId(string customerCode, string supplierId)
        {
            return await (from upd in dbContext.UnloadingPointDefault
                          where upd.CustomerCode == customerCode && upd.SupplierID == supplierId
                          select upd.DefaultUnloadingPointId).Cast<int?>().SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetPalletTypes<T>() where T : PalletType, new()
        {
            return await dbContext.PalletType
                .Select(x => new T()
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<T>> GetELLISPalletTypes<T>() where T : ELLISPalletType, new()
        {
            return await dbContext.ELLISPalletType
                .Select(x => new T()
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();
        }

        public PalletType GetPalletType(int Id)
        {
            return dbContext.PalletType.Find(Id);
        }

        public ELLISPalletType GetELLISPalletType(int Id)
        {
            return dbContext.ELLISPalletType.Find(Id);
        }

        #endregion

        #region add

        public async Task AddOutboundAsync(Outbound entity)
        {
            entity.CreatedDate = DateTime.Now;
            dbContext.Outbounds.Add(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task AddOutboundDetailAsync(OutboundDetail entity)
        {
            entity.CreatedDate = DateTime.Now;
            dbContext.OutboundDetails.Add(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task AddPickingListAsync(PickingList entity)
        {
            dbContext.PickingLists.Add(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task AddEKanbanHeaderAsync(EKanbanHeader entity)
        {
            entity.CreatedDate = DateTime.Now;
            dbContext.EKanbanHeaders.Add(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task AddEKanbanDetailAsync(EKanbanDetail entity)
        {
            dbContext.EKanbanDetails.Add(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task AddEStockTransferHeaderAsync(EStockTransferHeader entity)
        {
            var now = DateTime.Now;
            entity.IssuedDate = now;
            entity.CreatedDate = now;
            dbContext.EStockTransferHeaders.Add(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task AddEStockTransferDetailAsync(EStockTransferDetail entity)
        {
            dbContext.EStockTransferDetails.Add(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task AddEOrderAsync(EOrder entity)
        {
            dbContext.EOrders.Add(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task AddPickingListEKanbanAsync(PickingListEKanban entity)
        {
            dbContext.PickingListEKanbans.Add(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task AddPickingAllocatedPIDAsync(PickingAllocatedPID entity)
        {
            dbContext.PickingAllocatedPIDs.Add(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task AddInvTransactionAsync(InvTransaction entity, bool saveChanges = true)
        {
            var currentDate = DateTime.Now;
            entity.SystemDate = currentDate.Date;
            entity.SystemDateTime = currentDate;
            dbContext.InvTransactions.Add(entity);
            if (saveChanges)
                await dbContext.SaveChangesAsync();
        }
        public async Task AddInvTransactionPerWHSAsync(InvTransactionPerWHS entity, bool saveChanges = true)
        {
            var currentDate = DateTime.Now;
            entity.SystemDate = currentDate.Date;
            entity.SystemDateTime = currentDate;
            dbContext.InvTransactionsPerWHS.Add(entity);
            if (saveChanges)
                await dbContext.SaveChangesAsync();
        }
        public async Task AddInvTransactionPerSupplierAsync(InvTransactionPerSupplier entity, bool saveChanges = true)
        {
            var currentDate = DateTime.Now;
            entity.SystemDate = currentDate.Date;
            entity.SystemDateTime = currentDate;
            dbContext.InvTransactionsPerSupplier.Add(entity);
            if (saveChanges)
                await dbContext.SaveChangesAsync();
        }
        public async Task AddBillingLogAsync(BillingLog entity)
        {
            var date = DateTime.Now;
            entity.TransactionDate = date;
            entity.CreatedDate = date;
            dbContext.BillingLogs.Add(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task AddOutboundReleaseBondedLogAsync(OutboundReleaseBondedLog entity)
        {
            entity.ReleasedDate = DateTime.Now;
            dbContext.OutboundReleaseBondedLogs.Add(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task AddOutboundQRCodeAsync(OutboundQRCode entity)
        {
            entity.CreatedDate = DateTime.Now;
            dbContext.OutboundQRCodes.Add(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task AddPriceMasterAsync(PriceMaster entity)
        {
            entity.CreatedDate = DateTime.Now;
            dbContext.PriceMasters.Add(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task AddAccessLockAsync(AccessLock entity)
        {
            entity.LockedTime = DateTime.Now;
            dbContext.AccessLocks.Add(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task AddPickingListFixLogAsync(PickingListFixLog entity)
        {
            entity.FixedDate = DateTime.Now;
            dbContext.PickingListFixLogs.Add(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task AddPIDCodeAsync(PIDCode entity)
        {
            entity.CreatedDate = DateTime.Now;
            dbContext.PIDCodes.Add(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task BatchAddPIDCodeAsync(IEnumerable<PIDCode> entities)
        {
            foreach (var entity in entities)
            {
                entity.CreatedDate = DateTime.Now;
            }
            await dbContext.PIDCodes.AddRangeAsync(entities);
            await dbContext.SaveChangesAsync();
        }
        public async Task AddStorageDetailAsync(StorageDetail entity)
        {
            dbContext.StorageDetails.Add(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task AddStorageDetailGroupAsync(StorageDetailGroup entity)
        {
            dbContext.StorageDetailGroups.Add(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task BatchAddStorageDetailAsync(IEnumerable<StorageDetail> entities)
        {
            await dbContext.StorageDetails.AddRangeAsync(entities);
            await dbContext.SaveChangesAsync();
        }

        public async Task BatchAddExternalPIDAsync(IEnumerable<ExternalPID> entities, bool saveChanges = true)
        {
            await dbContext.ExternalPIDs.AddRangeAsync(entities);
            if (saveChanges)
                await dbContext.SaveChangesAsync();
        }

        public async Task AddErrorLogAsync(ErrorLog entity)
        {
            entity.CreatedDate = DateTime.Now;
            dbContext.ErrorLogs.Add(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task AddWHSTransferAsync(WHSTransfer entity)
        {
            var now = DateTime.Now;
            entity.CreatedDate = now;
            entity.ConfirmedDate = now;
            dbContext.WHSTransfers.Add(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task AddWHSTransferDetailAsync(WHSTransferDetail entity)
        {
            entity.TransferredDate = DateTime.Now;
            dbContext.WHSTransferDetails.Add(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task AddLocationAsync(Location entity)
        {
            entity.CreatedDate = DateTime.Now;
            dbContext.Locations.Add(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task AddShortfallCancelLogAsync(ShortfallCancelLog entity)
        {
            entity.CancelledDate = DateTime.Now;
            dbContext.ShortfallCancelLogs.Add(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task AddLoadingAsync(Loading entity)
        {
            entity.CreatedDate = DateTime.Now;
            dbContext.Loadings.Add(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task AddLoadingDetailAsync(LoadingDetail entity)
        {
            entity.AddedDate = DateTime.Now;
            dbContext.LoadingDetails.Add(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task AddReportPrintingLogAsync(ReportPrintingLog entity)
        {
            entity.PrintedDate = DateTime.Now;
            dbContext.ReportPrintingLogs.Add(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task AddInboundAsync(Inbound entity)
        {
            entity.CreatedDate = DateTime.Now;
            dbContext.Inbounds.Add(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task AddInboundDetailAsync(InboundDetail entity)
        {
            entity.CreatedDate = DateTime.Now;
            dbContext.InboundDetails.Add(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task BatchAddInboundDetailAsync(IEnumerable<InboundDetail> entities)
        {
            foreach (var entity in entities)
            {
                entity.CreatedDate = DateTime.Now;
            }
            await dbContext.InboundDetails.AddRangeAsync(entities);
            await dbContext.SaveChangesAsync();
        }

        public async Task AddQuarantineLogAsync(QuarantineLog entity)
        {
            entity.CreatedDate = DateTime.Now;
            dbContext.QuarantineLogs.Add(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task AddPartMasterAsync(PartMaster entity)
        {
            entity.CreatedDate = DateTime.Now;
            dbContext.PartMasters.Add(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task AddInventoryAsync(Inventory entity)
        {
            dbContext.Inventory.Add(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task AddItemMasterAsync(ItemMaster entity)
        {
            dbContext.ItemMasters.Add(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task AddStockTransferAsync(StockTransfer entity)
        {
            entity.CreatedDate = DateTime.Now;
            dbContext.StockTransfers.Add(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task AddStockTransferDetailAsync(StockTransferDetail entity)
        {
            entity.TransferredDate = DateTime.Now;
            dbContext.StockTransferDetails.Add(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task AddSunsetExpiredAlertAsync(SunsetExpiredAlert entity)
        {
            dbContext.SunsetExpiredAlerts.Add(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task AddSupplierItemMasterAsync(SupplierItemMaster entity)
        {
            dbContext.SupplierItemMasters.Add(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task AddUserAsync(User entity)
        {
            entity.CreatedDate = DateTime.Now;
            dbContext.Users.Add(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task AddAccessGroupAsync(AccessGroup entity)
        {
            entity.CreatedDate = DateTime.Now;
            dbContext.AccessGroups.Add(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task BatchAddAccessRightAsync(IEnumerable<AccessRight> entities)
        {
            dbContext.AccessRights.AddRange(entities);
            await dbContext.SaveChangesAsync();
        }

        public async Task BatchAddStockTransferDetailAsync(IEnumerable<StockTransferDetail> entities)
        {
            DateTime now = DateTime.Now;
            foreach (var entity in entities)
                entity.TransferredDate = now;
            dbContext.StockTransferDetails.AddRange(entities);
            await dbContext.SaveChangesAsync();
        }

        #endregion

        #region delete
        public async Task DeleteStorageDetailGroupAsync(StorageDetailGroup entity)
        {
            dbContext.StorageDetailGroups.Remove(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task DeleteOutboundAsync(Outbound entity)
        {
            dbContext.Outbounds.Remove(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task DeleteOutboundDetailAsync(OutboundDetail entity)
        {
            dbContext.OutboundDetails.Remove(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task DeleteLoadingDetailAsync(LoadingDetail entity)
        {
            dbContext.LoadingDetails.Remove(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task DeleteEKanbanDetailAsync(EKanbanDetail entity)
        {
            dbContext.EKanbanDetails.Remove(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task DeleteEOrderAsync(EOrder entity)
        {
            dbContext.EOrders.Remove(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task DeletePickingListAsync(PickingList entity)
        {
            dbContext.PickingLists.Remove(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task DeletePickingAllocatedPIDAsync(PickingAllocatedPID entity)
        {
            dbContext.PickingAllocatedPIDs.Remove(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task DeleteAccessLockAsync(AccessLock entity)
        {
            dbContext.AccessLocks.Remove(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteAccessLockByComputerName(string ComputerName)
        {
            await dbContext.AccessLocks.Where(x => x.ComputerName == ComputerName).ForEachAsync(a => dbContext.AccessLocks.Remove(a));
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteAllAccessLocksAsync()
        {
            await dbContext.AccessLocks.Where(x => x.Timeout.HasValue || x.ComputerName.StartsWith("TT:")).ForEachAsync(a => dbContext.AccessLocks.Remove(a));
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteTimeoutedLocksAsync()
        {
            await dbContext.AccessLocks.Where(x => x.Timeout.HasValue && x.LockedTime.Value.AddSeconds(x.Timeout.Value) < DateTime.Now).ForEachAsync(a => dbContext.AccessLocks.Remove(a));
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteAllAccessRightsAsync(string groupCode)
        {
            await dbContext.AccessRights.Where(r => r.GroupCode == groupCode).ForEachAsync(r => dbContext.AccessRights.Remove(r));
            await dbContext.SaveChangesAsync();
        }

        public async Task DeletePickingListAllocatedPIDAsync(PickingListAllocatedPID entity)
        {
            dbContext.PickingListAllocatedPIDs.Remove(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task DeletePickingListEKanbanAsync(PickingListEKanban entity)
        {
            dbContext.PickingListEKanbans.Remove(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task DeleteInboundDetailAsync(InboundDetail entity)
        {
            dbContext.InboundDetails.Remove(entity);
            await dbContext.SaveChangesAsync();
        }
        public async Task DeleteStockTransferDetailAsync(StockTransferDetail entity)
        {
            dbContext.StockTransferDetails.Remove(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task BatchDeleteStockTransferDetailAsync(IEnumerable<StockTransferDetail> entity)
        {
            dbContext.StockTransferDetails.RemoveRange(entity);
            await dbContext.SaveChangesAsync();
        }


        #endregion

        #region update

        public async Task<int> EKanbanHeaderBatchUpdateStatus(EKanbanStatus status, bool updateConfirmationDate,
            IEnumerable<string> ordernos, EKanbanStatus? currentStatus = null)
        {
            var orderNoParams = string.Join(",", Enumerable.Range(0, ordernos.Count()).Select(i => "{" + i + "}"));
            var values = ordernos.Cast<object>().ToList();

            var intStatus = (int)status;
            var sql = $"UPDATE EKanbanHeader SET Status = {intStatus} ";
            if (updateConfirmationDate)
            {
                sql += ", ConfirmationDate = GETDATE() ";
            }
            sql += $"WHERE OrderNo IN ({orderNoParams}) ";
            if (currentStatus.HasValue)
            {
                var currentStatusInt = (int)currentStatus;
                sql += $" AND Status = {currentStatusInt} ";
            }

            return await rawSqlExecutor.ExecuteSqlRawAsync(sql, values.ToArray());
        }

        public async Task<int> EKanbanDetailsBatchUpdateQtyReceived(params string[] ordernos)
        {
            var orderNoParams = string.Join(",", Enumerable.Range(0, ordernos.Count()).Select(i => "{" + i + "}"));
            var values = ordernos.Cast<object>().ToList();
            var sql = $"UPDATE EKanbanDetail SET QuantityReceived = QuantitySupplied WHERE OrderNo IN ({orderNoParams})";

            return await rawSqlExecutor.ExecuteSqlRawAsync(sql, values.ToArray());
        }

        #endregion
       
        public void ChangeTrackingOff() => dbContext.ChangeTrackingOff();

        public async Task SaveChangesAsync() => await dbContext.SaveChangesAsync();

        public async Task ExecuteQueryAsync(string query, Action<IDataReader> action, params KeyValuePair<string, object>[] parameters)
        {
            using var command = dbContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = query;
            foreach (var parameter in parameters)
            {
                AddParameter(command, parameter.Key, parameter.Value);
            }

            dbContext.Database.OpenConnection();
            using var result = await command.ExecuteReaderAsync();
            action(result);
        }

        private static void AddParameter(DbCommand command, string parameterName, object parameterValue)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = parameterValue;
            command.Parameters.Add(parameter);
        }

        private async Task<StorageDetailLineValuesQueryResult> GetStorageDetailInboundPriceValues(string jobno, int? lineItem)
        {
            var excludeStatuses = new StorageStatus[] { StorageStatus.Cancelled, StorageStatus.Splitted, StorageStatus.Decant };
            var lineDetails = await (from sd in dbContext.StorageDetails
                                     where sd.InJobNo == jobno
                                       && (!lineItem.HasValue || sd.LineItem == lineItem.Value)
                                       && !excludeStatuses.Contains(sd.Status)
                                     select new
                                     {
                                         LineValue = (sd.BuyingPrice ?? 0) * (sd.Qty != 0 ? sd.Qty : sd.OriginalQty),
                                         ResidualValue = sd.Qty != 0 ? (sd.BuyingPrice ?? 0) * sd.Qty : 0
                                     }).ToListAsync();
            return new StorageDetailLineValuesQueryResult
            {
                LineValue = lineDetails.Sum(i => i.LineValue),
                ResidualValue = lineDetails.Sum(i => i.ResidualValue)
            };
        }

        private static string FormatForWildcardSearch(string value)
            => value.EscapeForLikeExpr().Replace($"{EFCoreExtensions.ESCAPE_CHAR}%", "%");

        private class StorageDetailLineValuesQueryResult
        {
            public decimal LineValue { get; set; }
            public decimal ResidualValue { get; set; }
        }

        private readonly Context dbContext;
        private readonly IRawSqlExecutor rawSqlExecutor;
    }
}
