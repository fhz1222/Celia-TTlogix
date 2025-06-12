using Application.Common.Models;
using Application.Interfaces.Repositories;
using Application.UseCases.Storage.Queries.GetPalletsForILogStockDiscrepancyReport;
using Application.UseCases.Storage.Queries.GetILogInboundPalletsWithTypeQuery;
using Application.UseCases.Storage.Queries.GetStorageDetailItems;
using Application.UseCases.StorageDetails;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;
using Persistence.Extensions;
using LocationType = Application.Common.Enums.LocationType;
using OutboundType = Application.Common.Enums.OutboundType;
using EFCore = Persistence.EFCoreExtensions;
using AutoMapper.QueryableExtensions;
using Application.UseCases.Registration.Commands.UpdateLocation;
using System.Text;

namespace Persistence.Repositories;
public class StorageDetailRepository : IStorageDetailRepository
{
    private readonly AppDbContext context;
    private readonly AutoMapper.IMapper mapper;

    public StorageDetailRepository(AppDbContext context, AutoMapper.IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public StorageDetailPaginatedList GetStorageDetailItems(PaginationQuery pagination,
                                                             StorageDetailItemDtoFilter filter,
                                                             string? orderBy,
                                                             bool orderByDescending)
    {
        // sub-query: for CommInvNo
        var stockTransferData = context.TtStockTransferDetails
            .Join(context.TtStockTransfers
                .Where(st => st.Status == (byte) StockTransferStatus.Completed
                    && !context.TtStfreversalMasters.Any(srm => srm.Status == (byte) StockTransferReversalStatus.Completed
                        && srm.StfjobNo == st.JobNo)
                ),
            stockTransferDetail => stockTransferDetail.JobNo,
            stockTransfer => stockTransfer.JobNo,
            (stockTransferData, stockTransfer) => new { CommInvNo = stockTransfer.CommInvNo, Pid = stockTransferData.Pid });

        var mainQuery = context.TtStorageDetails
            .AsNoTracking()
            .Join(context.TtPartMasters,
                   storageDetail => new { CustomerCode = storageDetail.CustomerCode ?? "", storageDetail.SupplierId, storageDetail.ProductCode },
                   partmaster => new { partmaster.CustomerCode, partmaster.SupplierId, ProductCode = partmaster.ProductCode1 },
                   (storageDetail, partmaster) => storageDetail)
            .Join(context.TtLocations,
                storageDetail => new { storageDetail.LocationCode, storageDetail.Whscode },
                location => new { LocationCode = location.Code, location.Whscode },
                (storageDetail, location) => new { storageDetail = storageDetail, Location = location })
            .Join(context.ILogLocationCategories,
                x => new { Id = x.Location.ILogLocationCategoryId },
                cat => new { cat.Id },
                (x, cat) => new { storageDetail = x.storageDetail, Location = x.Location.Code, ILogLocCategory = cat.Name })
            .Join(context.TtInbounds,
                data => new { JobNo = data.storageDetail.InJobNo },
                inbound => new { inbound.JobNo },
                (data, inbond) => new { StorageDetail = data.storageDetail, data.Location, inbond.RefNo, data.ILogLocCategory })
            // internal condition
            .Where(sd => sd.StorageDetail.Status != (byte)StorageStatus.Cancelled && sd.StorageDetail.Status != (byte)StorageStatus.DiscrepancyFixed)
            // apply external filters
            .Where(sd => sd.StorageDetail.Whscode == filter.WhsCode
                && (filter.CustomerCode == null || sd.StorageDetail.CustomerCode == filter.CustomerCode)
                && (filter.LineItem == null || (sd.StorageDetail.LineItem >= filter.LineItem.From && sd.StorageDetail.LineItem <= filter.LineItem.To))
                && (filter.SequenceNo == null || (sd.StorageDetail.SeqNo >= filter.SequenceNo.From && sd.StorageDetail.SeqNo <= filter.SequenceNo.To))
                && (filter.Location == null || sd.Location == filter.Location)
                && (filter.ProductCode == null || sd.StorageDetail.ProductCode == filter.ProductCode)
                && (filter.Ownership == null || sd.StorageDetail.Ownership == filter.Ownership)
                && (filter.Status == null || sd.StorageDetail.Status == filter.Status)
                && (filter.InboundDate == null || (sd.StorageDetail.InboundDate >= filter.InboundDate.From && sd.StorageDetail.InboundDate <= filter.InboundDate.To))
                && (filter.Qty == null || (sd.StorageDetail.Qty >= filter.Qty.From && sd.StorageDetail.Qty <= filter.Qty.To))
                && (filter.AllocatedQty == null || (sd.StorageDetail.AllocatedQty >= filter.AllocatedQty.From && sd.StorageDetail.AllocatedQty <= filter.AllocatedQty.To))
                && (filter.BondedStatus == null || sd.StorageDetail.BondedStatus == Convert.ToByte(filter.BondedStatus)));

        mainQuery = filter.PID switch
        {
            null => mainQuery,
            string p when p.Contains('%') => mainQuery.Where(sd => EF.Functions.Like(sd.StorageDetail.Pid, FormatForWildcardSearch(p), EFCore.ESCAPE_CHAR)),
            string p => mainQuery.Where(sd => sd.StorageDetail.Pid == p)
        };
        mainQuery = filter.SupplierId switch
        {
            null => mainQuery,
            string s when s.Contains('%') => mainQuery.Where(sd => EF.Functions.Like(sd.StorageDetail.SupplierId, FormatForWildcardSearch(s), EFCore.ESCAPE_CHAR)),
            string s => mainQuery.Where(sd => sd.StorageDetail.SupplierId == s)
        };
        mainQuery = filter.InboundJobNo switch
        {
            null => mainQuery,
            string i when i.Contains('%') => mainQuery.Where(sd => EF.Functions.Like(sd.StorageDetail.InJobNo, FormatForWildcardSearch(i), EFCore.ESCAPE_CHAR)),
            string i => mainQuery.Where(sd => sd.StorageDetail.InJobNo == i)
        };
        mainQuery = filter.OutboundJobNo switch
        {
            null => mainQuery,
            string o when o.Contains('%') => mainQuery.Where(sd => EF.Functions.Like(sd.StorageDetail.OutJobNo, FormatForWildcardSearch(o), EFCore.ESCAPE_CHAR)),
            string o => mainQuery.Where(sd => sd.StorageDetail.OutJobNo == o)
        };
        mainQuery = filter.RefNo switch
        {
            null => mainQuery,
            string r when r.Contains('%') => mainQuery.Where(sd => EF.Functions.Like(sd.RefNo, FormatForWildcardSearch(r), EFCore.ESCAPE_CHAR)),
            string r => mainQuery.Where(sd => sd.RefNo == r)
        };
        mainQuery = filter.ParentId switch
        {
            null => mainQuery,
            string p when p.Contains('%') => mainQuery.Where(sd => EF.Functions.Like(sd.StorageDetail.ParentId, FormatForWildcardSearch(p), EFCore.ESCAPE_CHAR)),
            string p => mainQuery.Where(sd => sd.StorageDetail.ParentId == p)
        };

        // left outer join with external pids
        var queryWithExtPid = mainQuery
            .GroupJoin(context.TtExternalPids,
                sd => sd.StorageDetail.Pid,
                extPids => extPids.Pid,
                (sd, extPids) => new { mainData = sd, extPids = extPids })
            .SelectMany(
                groupedData => groupedData.extPids.DefaultIfEmpty(),
                (sd, extPid) => new
                {
                    StorageDetail = sd.mainData.StorageDetail,
                    Location = sd.mainData.Location,
                    RefNo = sd.mainData.RefNo,
                    ExternalPid = extPid == null ? null : extPid.ExternalPid,
                    sd.mainData.ILogLocCategory
                });
        queryWithExtPid = filter.ExternalPID switch
        {
            null => queryWithExtPid,
            string e when e.Contains('%') => queryWithExtPid.Where(sd => EF.Functions.Like(sd.ExternalPid, FormatForWildcardSearch(e), EFCore.ESCAPE_CHAR)),
            string e => queryWithExtPid.Where(sd => sd.ExternalPid == e)
        };

        // left outer join with stock transfer data 
        var queryWithStf = queryWithExtPid
            .GroupJoin(stockTransferData,
                sd => sd.StorageDetail.Pid,
                st => st.Pid,
                (sd, stData) => new { mainData = sd, stockTransfers = stData })
            .SelectMany(
                groupedData => groupedData.stockTransfers.DefaultIfEmpty(),
                (sd, stData) => new
                {
                    StorageDetail = sd.mainData.StorageDetail,
                    Location = sd.mainData.Location,
                    RefNo = sd.mainData.RefNo,
                    ExternalPid = sd.mainData.ExternalPid,
                    CommInvNo = stData == null ? null : stData.CommInvNo,
                    sd.mainData.ILogLocCategory
                });
        queryWithStf = filter.CommInvNo switch
        {
            null => queryWithStf,
            string c when c.Contains('%') => queryWithStf.Where(sd => sd.CommInvNo != null && EF.Functions.Like(sd.CommInvNo, FormatForWildcardSearch(c), EFCore.ESCAPE_CHAR)),
            string c => queryWithStf.Where(sd => sd.CommInvNo != null && sd.CommInvNo == c)
        };

        var result = queryWithStf;

        orderBy ??= "pid";
        var querableResult = result.Select(r => new { r.StorageDetail, r.Location, r.ExternalPid, r.RefNo, r.CommInvNo, r.ILogLocCategory })
            .OrderByDescOrAsc(orderByDescending, (i) =>
                orderBy.ToLower() == "pid" ? i.StorageDetail.Pid :
                orderBy.ToLower() == "supplierid" ? i.StorageDetail.SupplierId :
                orderBy.ToLower() == "customercode" ? i.StorageDetail.CustomerCode :
                orderBy.ToLower() == "productcode" ? i.StorageDetail.ProductCode :
                orderBy.ToLower() == "ownership" ? ((int)i.StorageDetail.Ownership) :
                orderBy.ToLower() == "location" ? i.Location :
                orderBy.ToLower() == "externalpid" ? i.ExternalPid :
                orderBy.ToLower() == "inboundjobno" ? i.StorageDetail.InJobNo :
                orderBy.ToLower() == "outboundjobno" ? i.StorageDetail.OutJobNo :
                orderBy.ToLower() == "lineitem" ? i.StorageDetail.LineItem :
                orderBy.ToLower() == "sequenceno" ? i.StorageDetail.SeqNo :
                orderBy.ToLower() == "controlcode1" ? i.StorageDetail.ControlCode1 :
                orderBy.ToLower() == "inbounddate" ? i.StorageDetail.InboundDate :
                orderBy.ToLower() == "controlcode2" ? i.StorageDetail.ControlCode2 :
                orderBy.ToLower() == "parentid" ? i.StorageDetail.ParentId :
                orderBy.ToLower() == "status" ? ((byte)i.StorageDetail.Status) :
                orderBy.ToLower() == "bondedstatus" ? ((int)i.StorageDetail.BondedStatus) :
                orderBy.ToLower() == "refno" ? i.RefNo :
                orderBy.ToLower() == "comminvno" ? i.CommInvNo :
                orderBy.ToLower() == "qty" ? i.StorageDetail.Qty :
                orderBy.ToLower() == "allocatedqty" ? i.StorageDetail.AllocatedQty :
                i.StorageDetail.Pid); 

        var items = querableResult.Skip((pagination.PageNumber - 1) * pagination.ItemsPerPage).Take(pagination.ItemsPerPage).ToList()
            .Select(r => mapper.Map<StorageDetailItemDto>(r.StorageDetail).SetExternalValues(r.Location,
                r.ExternalPid,
                r.RefNo,
                r.CommInvNo,
                r.ILogLocCategory)).ToList();

        var count = result.Count();
        var totalQty = result.Sum(r => r.StorageDetail.Qty);

        return new StorageDetailPaginatedList(items, count, pagination.PageNumber, pagination.ItemsPerPage, totalQty);
    }


    public async Task<PaginatedList<StorageDetailItemWithPartInfoDto>> GetStorageDetailWithPartsForOutJobNoAndLine(PaginationQuery pagination, StorageDetailItemWithPartInfoDtoFilter filter,
                                                             string? orderBy,
                                                             bool orderByDescending)
    {
        var outbound = await context.FindAsync<TtOutbound>(filter.OutboundJobNo);
        var outboundDetail = await context.FindAsync<TtOutboundDetail>(filter.OutboundJobNo, filter.LineItem);

        if (outbound == null || outboundDetail == null)
            throw new Exception();

        var customerCode = outbound.CustomerCode;
        var productCode = outboundDetail.ProductCode;
        var supplierId = outboundDetail.SupplierID;
        int? ownership = outbound.TransType == (int)OutboundType.Return ? (int)Ownership.Supplier : null;

        var query = from sd in context.TtStorageDetails.AsNoTracking()
        join pm in context.TtPartMasters on new { sd.CustomerCode, sd.SupplierId, sd.ProductCode }
                                equals new { pm.CustomerCode, pm.SupplierId, ProductCode = pm.ProductCode1 }
                    join location in context.TtLocations.DefaultIfEmpty() on new { sd.LocationCode, sd.Whscode }
                                equals new { LocationCode = location.Code, location.Whscode }
                    join inbound in context.TtInbounds.DefaultIfEmpty() on sd.InJobNo equals inbound.JobNo
                    join externalPID in context.TtExternalPids.DefaultIfEmpty() on sd.Pid equals externalPID.Pid into ext
                    from externalP in ext.DefaultIfEmpty()
                    join uomDec in context.TtUOMDecimals.DefaultIfEmpty() on new { pm.CustomerCode, UOM = pm.Uom }
                            equals new { uomDec.CustomerCode, uomDec.UOM } into uomD
                    from uomdecimal in uomD.Where(d => d.Status == 1).DefaultIfEmpty()

                    where pm.CustomerCode == customerCode
                        && pm.ProductCode1 == productCode
                        && pm.SupplierId == supplierId
                        && location.Type == (byte)LocationType.Normal
                        && sd.Whscode == filter.WhsCode
                        && (filter.GroupID == null || sd.GroupId == filter.GroupID)
                        && !String.IsNullOrEmpty(sd.LocationCode)
                        && String.IsNullOrEmpty(sd.OutJobNo)
                        && sd.Status == (int)StorageStatus.Putaway

                    orderby sd.Ownership descending, sd.InboundDate ascending, // FIFO
                        location.IsPriority descending,
                        (sd.PutawayDate.HasValue ? sd.PutawayDate.Value.Date : new DateTime?()) ascending,
                        sd.InJobNo ascending, sd.Version, sd.LocationCode, sd.Pid

                    select new StorageDetailItemWithPartInfoDto()
                    {
                        PID = sd.Pid,
                        ProductCode = sd.ProductCode,
                        SupplierID = sd.SupplierId,
                        Qty = sd.Qty,
                        GroupID = sd.GroupId,
                        InboundDate = sd.InboundDate,
                        LocationCode = sd.LocationCode,
                        Ownership = sd.Ownership,
                        WHSCode = filter.WhsCode,
                        DecimalNum = uomdecimal != null ? uomdecimal.DecimalNum : 0,
                        ExternalPID = externalP.ExternalPid,
                        RefNo = inbound.RefNo,
                        SPQ = pm.Spq
                    };

        if (ownership.HasValue)
        {
            query = query.Where(q => q.Ownership == (Ownership)ownership.Value);
        }
        if (filter.PID != null)
        {
            query = query.Where(q => EF.Functions.Like(q.PID, filter.PID.FormatForLikeExpr(), EFCore.ESCAPE_CHAR));
        }
        if (filter.Location != null)
        {
            query = query.Where(q => EF.Functions.Like(q.LocationCode, filter.Location.FormatForLikeExpr(), EFCore.ESCAPE_CHAR));
        }
        if (filter.InboundDate.HasValue)
        {
            var inboundDate = filter.InboundDate.Value.Date;
            query = query.Where(q => q.InboundDate.Date == inboundDate);
        }
        if (filter.GroupID != null)
        {
            query = query.Where(q => EF.Functions.Like(q.GroupID, filter.GroupID.FormatForLikeExpr(), EFCore.ESCAPE_CHAR));
        }
        orderBy ??= "PID".ToLower();
        var orderedQuery = query.OrderByDescOrAsc(orderByDescending, (i) =>
                orderBy.ToLower() == nameof(i.PID).ToLower() ? i.PID :
                orderBy.ToLower() == nameof(i.Qty).ToLower() ? i.Qty :
                orderBy.ToLower() == nameof(i.InboundDate).ToLower() ? i.InboundDate :
                orderBy.ToLower() == nameof(i.Ownership).ToLower() ? i.Ownership :
                orderBy.ToLower() == nameof(i.LocationCode).ToLower() ? i.LocationCode :
                orderBy.ToLower() == nameof(i.GroupID).ToLower() ? i.GroupID : i.PID);


        var items = await orderedQuery
            .Skip((pagination.PageNumber - 1) * pagination.ItemsPerPage)
            .Take(pagination.ItemsPerPage).ToListAsync();
        var count = query.Select(r => r.PID).Count();

        return new PaginatedList<StorageDetailItemWithPartInfoDto>(items, count, pagination.PageNumber, pagination.ItemsPerPage);
    }

    public Pallet? GetPalletDetail(string palletId)
    {
        var storageDetail = context.TtStorageDetails.Find(palletId);
        if (storageDetail == null)
            return null;
        var result = mapper.Map<Pallet>(storageDetail);
        // map product details
        var partMaster = context.TtPartMasters.Find(storageDetail.CustomerCode, storageDetail.SupplierId, storageDetail.ProductCode);
        result.Product = mapper.Map<Product>(partMaster);
        return result;
    }

    public async Task Update(Pallet pallet)
    {
        var existingObject = await context.TtStorageDetails.FindAsync(pallet.Id) ?? throw new EntityDoesNotExistException();
        existingObject.Qty = pallet.Qty;
        existingObject.QtyPerPkg = pallet.QtyPerPkg;
        existingObject.AllocatedQty = pallet.AllocatedQty;
        existingObject.Status = (byte) pallet.Status;
        existingObject.LocationCode = pallet.Location;
        existingObject.Whscode = pallet.WhsCode;
        existingObject.OutJobNo = pallet.OutboundJobNo;
        existingObject.Ownership = (byte)pallet.Ownership;
        existingObject.IsVmi = (byte)(pallet.IsVmi ? 1 : 0);
        existingObject.PutawayBy = pallet.PutawayBy;
        existingObject.PutawayDate = pallet.PutawayDate;
    }

    public string? GetLastPIDNumber(string prefix)
    {
        return context.TtPidCodes.Where(pc => pc.PidNo.StartsWith(prefix)).Max(pc => pc.PidNo);
    }

    public void AddNewPIDCode(string newPid, DateTime createdDate)
    {
        context.Add(new Entities.TtPidCode
        {
            PidNo = newPid,
            CreatedDate = createdDate
        });
    }

    public async Task AddNewPallet(Pallet newPallet, string origPalletId)
    {
        // create new ttStorageDetail object
        var newStorageDetail = mapper.Map<TtStorageDetail>(newPallet);
        // copy fields from the original ttStorageDetail to the new ttStorageDetail object

        var originalStorageDetail = await context.FindAsync<TtStorageDetail>(origPalletId);
        if (originalStorageDetail == null)
            throw new EntityDoesNotExistException();
        newStorageDetail.CopyValuesFromParent(originalStorageDetail);
        context.Add(newStorageDetail);
    }

    public IEnumerable<ILogInboundPalletWithTypeDto> GetILogInboundPalletsWithType(ILogInboundPalletsWithTypeDtoFilter filter, int iLogInboundCategoryId)
    {
        var locations = context.TtLocations
            .AsNoTracking()
            .Where(lc => lc.ILogLocationCategoryId == iLogInboundCategoryId);

        var results = context.TtStorageDetails
            .AsNoTracking()
            .Join(context.TtPartMasters.AsNoTracking(),
                sd => new { CustomerCode = sd.CustomerCode ?? "", sd.SupplierId, sd.ProductCode },
                pm => new { pm.CustomerCode, pm.SupplierId, ProductCode = pm.ProductCode1 },
                (sd, pm) => new { sd, pm.PalletTypeId, pm.IsCpart })
            .Join(context.TtPalletTypes.AsNoTracking(),
                obj => obj.PalletTypeId,
                pt => pt.Id,
                (obj, pt) => new { obj.sd, PalletType = pt.Name, obj.IsCpart })
            .GroupJoin(context.TtExternalPids.AsNoTracking(),
                obj => obj.sd.Pid,
                ep => ep.Pid,
                (obj, eps) => new { obj.sd, obj.PalletType, obj.IsCpart, eps })
            .SelectMany(
                obj => obj.eps.DefaultIfEmpty(),
                (obj, ep) => new { obj.sd, obj.PalletType, obj.IsCpart, ep })
            .Where(row => locations.Any(x => x.Code == row.sd.LocationCode && x.Whscode == row.sd.Whscode))
            .OptionalWhere(filter.PIDs, PIDs => row => PIDs.Contains(row.sd.Pid))
            .OptionalWhere(filter.JobNo, jobNo => row => jobNo == row.sd.InJobNo)
            .Select(row => new ILogInboundPalletWithTypeDto
            {
                LocationCode = row.sd.LocationCode,
                PID = row.sd.Pid,
                PalletType = row.PalletType,
                IsCPart = row.IsCpart == 1,
                Height = row.sd.Height,
                Width = row.sd.Width,
                Length = row.sd.Length,
                ProductCode = row.sd.ProductCode,
                InboundDate = row.sd.InboundDate,
                Ownership = row.sd.Ownership,
                // pallet in inbound always has PutawayDate
                PutawayDate = (DateTime)row.sd.PutawayDate!,
                IsQuarantine = row.sd.Status == (byte)PaletStatus.Quarantine,
                CustomerCode = row.sd.CustomerCode!,
                SupplierId = row.sd.SupplierId!,
                Qty = row.sd.Qty,
                ExternalPID = row.ep != null ? row.ep.ExternalPid : null
            });
        return results;
    }

    public IEnumerable<StockDiscrepancyReportPalletDto> GetPalletsForILogStockDiscrepancyReport(StockDiscrepancyReportPalletDtoFilter filter, int iLogStorageCategoryId)
    {
        var results = GetNotDispatchedILogStoragePalletsWithQty(filter.WHSCodes, iLogStorageCategoryId)
            .Select(sd => new StockDiscrepancyReportPalletDto
            {
                PID = sd.Pid,
                Qty = sd.Qty,
                Ownership = sd.Ownership,
                LocationCode = sd.LocationCode,
                ProductCode = sd.ProductCode,
                CustomerCode = sd.CustomerCode!,
            });
        return results;
    }

    public List<ILogStockSynchronizationPalletDto> GetILogStockSynchronizationData(string[] WHSCodes, int iLogStorageCategoryId)
    {
        var results = GetNotDispatchedILogStoragePalletsWithQty(WHSCodes, iLogStorageCategoryId)
            .Join(context.TtPartMasters.AsNoTracking(),
                sd => new { CustomerCode = sd.CustomerCode ?? "", sd.SupplierId, sd.ProductCode },
                pm => new { pm.CustomerCode, pm.SupplierId, ProductCode = pm.ProductCode1 },
                (sd, pm) => new { sd, pm })
            .GroupJoin(context.TtExternalPids.AsNoTracking(),
                obj => obj.sd.Pid,
                ep => ep.Pid,
                (obj, eps) => new { obj.sd, obj.pm, eps })
            .SelectMany(
                obj => obj.eps.DefaultIfEmpty(),
                (obj, ep) => new { obj.sd, obj.pm, ep })
            .Select(d => new ILogStockSynchronizationPalletDto
            {
                PID = d.sd.Pid,
                ProductCode = d.sd.ProductCode,
                Quantity = (int)d.sd.Qty,
                LocationCode = d.sd.LocationCode,
                Quarantine = (d.sd.Status == (byte)PaletStatus.Quarantine) || (d.sd.Status == (byte)PaletStatus.Transferring) ? 1 : 0,
                Ownership = d.sd.Ownership,
                InboundDate = d.sd.InboundDate.ToString("dd/MM/yyyy"),
                CustomerCode = d.sd.CustomerCode!,
                SupplierID = d.sd.SupplierId,
                IsCpart = d.pm.IsCpart == 1,
                CPartBoxQty = (int)d.pm.CpartSpq,
                ExternalPID = d.ep != null ? d.ep.ExternalPid : null
            });
        return results.ToList();
    }

    public List<Pallet> GetPallets(string[] pids)
    {
        var query = context.TtStorageDetails.Where(s => pids.Contains(s.Pid));
        var result = query.ProjectTo<Pallet>(mapper.ConfigurationProvider).ToList();
        return result;
    }

    public List<string> GetPidsInILog(string[] pids, string[] whsCodes, int storageCategoryId)
    {
        return GetNotDispatchedILogStoragePalletsWithQty(whsCodes, storageCategoryId)
            .Where(p => pids.Contains(p.Pid))
            .Select(p => p.Pid)
            .ToList();
    }

    private IQueryable<TtStorageDetail> GetNotDispatchedILogStoragePalletsWithQty(string[] WHSCodes, int iLogStorageCategoryId)
    {
        var locations = context.TtLocations
            .AsNoTracking()
            .Where(l => WHSCodes.Contains(l.Whscode))
            .Where(l => l.ILogLocationCategoryId == iLogStorageCategoryId)
            .Select(l => new { Code = l.Code, WhsCode = l.Whscode });

        var results = context.TtStorageDetails
            .AsNoTracking()
            .Where(sd => sd.Status != (byte)PaletStatus.Dispatched)
            .Where(sd => sd.Qty > 0)
            .Join(locations,
                sd => new { Code = sd.LocationCode, WhsCode = sd.Whscode },
                l => l,
                (sd, l) => sd);
        return results;
    }

    public List<StorageStatus> GetPalletStatusesOnLocationForRegistrationUpdateLocation(string locationCode, string locationWhsCode)
    {
        var results = context.TtStorageDetails
            .AsNoTracking()
            .Where(sd => sd.Status != (byte)PaletStatus.Cancelled)
            .Where(sd => sd.Status != (byte)PaletStatus.InTransit)
            .Where(sd => sd.Status != (byte)PaletStatus.Dispatched)
            .Where(sd => sd.Status != (byte)PaletStatus.DiscrepancyFixed)
            .Where(sd => sd.LocationCode == locationCode)
            .Where(sd => sd.Whscode == locationWhsCode)
            .Select(x => (StorageStatus)x.Status)
            .ToList();
        return results;
    }

    public int GetPalletCountOnLocationForRegistrationToggleActiveLocation(string locationCode, string locationWhsCode)
    {
        var statuses = new int[]
        {
            StorageStatus.Incoming,
            StorageStatus.Putaway,
            StorageStatus.Picked,
            StorageStatus.Packed,
            StorageStatus.Allocated,
            StorageStatus.Quarantine,
        };
        var count = context.TtStorageDetails
            .AsNoTracking()
            .Where(sd => statuses.Contains(sd.Status))
            .Where(sd => sd.LocationCode == locationCode)
            .Where(sd => sd.Whscode == locationWhsCode)
            .Where(sd => sd.Qty != 0)
            .Count();
        return count;
    }

    private string FormatForWildcardSearch(string? value) 
        => value?.FormatForLikeExpr().Replace($"{EFCore.ESCAPE_CHAR}%", "%") ?? string.Empty;
}
