using Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using EFCore = Persistence.EFCoreExtensions;
using Application.UseCases.StockTransferReversals;
using Application.UseCases.StockTransferReversals.Queries.GetStockTransferReversals;
using AutoMapper.QueryableExtensions;
using Application.Interfaces.Repositories;
using Domain.Enums;
using Domain.Entities;
using Persistence.Entities;
using Application.UseCases.StockTransferReversalItems;
using Application.UseCases.StockTransferReversalItems.Commands.AddNewStockTransferReversalItemCommand;
using Domain.ValueObjects;
using Application.UseCases.StockTransferReversals.Commands.CompleteStockTransferReversalCommand;

namespace Persistence.Repositories;

public class StockTransferReversalRepository : IStockTransferReversalRepository
{
    private readonly AppDbContext context;
    private readonly AutoMapper.IMapper mapper;

    public CodePrefix GetCodePrefix => CodePrefix.StockTransferReversal;

    public int GetLastJobNumber(string prefix)
    {
        var jobNo = context.TtStfreversalMasters
            .AsNoTracking()
            .Where(x => EF.Functions.Like(x.JobNo, $"{prefix}%"))
            .OrderByDescending(x => x.JobNo)
            .Select(x => x.JobNo)
            .FirstOrDefault();

        var no = jobNo is null ? 0 : Convert.ToInt32(jobNo[^5..]);
        return no;
    }

    public StockTransferReversalRepository(AppDbContext context, AutoMapper.IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public PaginatedList<StockTransferReversalDto> GetStockTransferReversals(
        PaginationQuery pagination,
        GetStockTransferReversalsDtoFilter filter,
        string? orderBy,
        bool orderByDescending)
    {
        var query = context.TtStfreversalMasters
            .AsNoTracking()
            .OptionalWhere(filter.CreatedDate, createdDate => row => row.CreatedDate >= createdDate.From && row.CreatedDate <= createdDate.To);

        if(filter.Statuses != null && filter.Statuses.Any())
        {
            query = query.Where(ir => filter.Statuses.Contains(ir.Status));
        }

        query = filter.JobNo switch
        {
            null => query,
            string s => query.Where(ir => ir.JobNo == s)
        };
        query = filter.WhsCode switch
        {
            null => query,
            string s => query.Where(ir => ir.Whscode == s)
        };
        query = filter.CustomerCode switch
        {
            null => query,
            string s => query.Where(ir => ir.CustomerCode == s)
        };
        query = filter.RefNo switch
        {
            null => query,
            string s => query.Where(ir => ir.RefNo == s)
        };
        query = filter.Reason switch
        {
            null => query,
            string s when s.Contains('%') => query.Where(ir => ir.Reason != null && EF.Functions.Like(ir.Reason, FormatForWildcardSearch(s), EFCore.ESCAPE_CHAR)),
            string s => query.Where(ir => ir.Reason == s)
        };

        orderBy ??= "jobno";
        var querableResult = query
            .OrderByDescOrAsc(orderByDescending, (i) =>
                orderBy.ToLower() == "jobno" ? i.JobNo :
                orderBy.ToLower() == "whscode" ? i.Whscode :
                orderBy.ToLower() == "customercode" ? i.CustomerCode :
                orderBy.ToLower() == "refno" ? i.RefNo :
                orderBy.ToLower() == "reason" ? i.Reason :
                orderBy.ToLower() == "createddate" ? i.CreatedDate :
                orderBy.ToLower() == "status" ? i.Status :
                i.JobNo);

        var items = querableResult
            .Skip((pagination.PageNumber - 1) * pagination.ItemsPerPage)
            .Take(pagination.ItemsPerPage)
            .ProjectTo<StockTransferReversalDto>(mapper.ConfigurationProvider)
            .ToList();

        var count = query.Count();

        return new PaginatedList<StockTransferReversalDto>(items, count, pagination.PageNumber, pagination.ItemsPerPage);
    }

    public async Task<StockTransferInfo?> GetStockTransferInfo(string jobNo)
    {
        var stf = await context.TtStockTransfers.FindAsync(jobNo);

        return stf is null ? null : new StockTransferInfo
        {
            CustomerCode = stf.CustomerCode,
            RefNo = stf.RefNo,
            WhsCode = stf.WhsCode,
            Status = (StockTransferStatus)stf.Status,
            Type = (StockTransferType)stf.TransferType,
        };
    }

    public async Task AddNew(StockTransferReversal newStockTransferReversal)
    {
        var reversal = mapper.Map<TtStfReversalMaster>(newStockTransferReversal);

        context.TtStfreversalMasters.Add(reversal);
        await context.SaveChangesAsync();
    }

    public PaginatedList<ReversibleStockTransferDto> GetReversibleStockTransfers(PaginationQuery pagination, string whsCode, string? stfJobNo, DateTime? newerThan)
    {
        var query = context.TtStockTransfers
            .AsNoTracking()
            .Where(x => x.WhsCode == whsCode)
            .Where(x => x.Status == (byte)StockTransferStatus.Completed)
            .Where(x => x.TransferType == (byte)StockTransferType.Over90Days || x.TransferType == (byte)StockTransferType.EStockTransfer)
            .OptionalWhere(stfJobNo, jobNo => row => row.JobNo == jobNo)
            .OptionalWhere(newerThan, date => row => row.CreatedDate >= date)
            .OrderByDescending(x => x.JobNo);

        var stockTransfers = query
            .Skip((pagination.PageNumber - 1) * pagination.ItemsPerPage)
            .Take(pagination.ItemsPerPage)
            .Select(x => new ReversibleStockTransferDto
            {
                JobNo = x.JobNo,
                RefNo = x.RefNo ?? ""
            })
            .ToList();

        var count = query.Count();

        return new PaginatedList<ReversibleStockTransferDto>(stockTransfers, count, pagination.PageNumber, pagination.ItemsPerPage);
    }

    public async Task<StockTransferReversal?> GetStockTransferReversal(string jobNo)
    {
        var reversal = await context.TtStfreversalMasters.FindAsync(jobNo);
        return mapper.Map<StockTransferReversal>(reversal);
    }

    public async Task<bool> AnyStockTransferReversalDetailsExists(string jobNo)
    {
        var result = await context.TtStfreversalDetails
            .Where(x => x.JobNo == jobNo)
            .AnyAsync();

        return result;
    }

    public async Task Update(StockTransferReversal updated)
    {
        var existing = await context.FindAsync<TtStfReversalMaster>(updated.JobNo)
            ?? throw new EntityDoesNotExistException();

        mapper.Map(updated, existing);
    }

    public async Task<string?> GetCustomerName(string customerCode, string whsCode)
    {
        var customer = await context.TtCustomers.FindAsync(customerCode, whsCode);
        return customer?.Name;
    }

    public List<StockTransferReversalItemDto> GetStockTransferReversalItems(string jobNo, string? orderBy, bool orderByDescending)
    {
        var query = context.TtStfreversalDetails
            .AsNoTracking()
            .Join(
                context.TtStorageDetails,
                detail => detail.Pid,
                sd => sd.Pid,
                (detail, sd) => new { detail, sd })
            .Join(
                context.TtPartMasters,
                outer => new { CustomerCode = outer.sd.CustomerCode ?? "", outer.sd.ProductCode, outer.sd.SupplierId },
                pm => new { pm.CustomerCode, ProductCode = pm.ProductCode1, pm.SupplierId },
                (outer, pm) => new { outer.detail, outer.sd, pm })
            .Where(x => x.detail.JobNo == jobNo)
            .Select(x => new StockTransferReversalItemDto
            {
                PID = x.detail.Pid,
                OriginalSupplierID = x.detail.OriginalSupplierId,
                ProductCode = x.detail.ProductCode,
                Description = x.pm.Description,
                Qty = (int)x.sd.Qty,
                NewWhsCode = x.detail.Whscode,
                NewLocationCode = x.detail.LocationCode
            });

        orderBy ??= "pid";
        query = query
            .OrderByDescOrAsc(orderByDescending, (i) =>
                orderBy.ToLower() == "pid" ? i.PID :
                orderBy.ToLower() == "originalsupplierid" ? i.OriginalSupplierID :
                orderBy.ToLower() == "productcode" ? i.ProductCode :
                orderBy.ToLower() == "description" ? i.Description :
                orderBy.ToLower() == "qty" ? i.Qty :
                orderBy.ToLower() == "newwhscode" ? i.NewWhsCode :
                orderBy.ToLower() == "newlocationcode" ? i.NewLocationCode :
                i.PID);

        var items = query.ToList();

        return items;
    }

    public async Task DeleteDetail(string jobNo, string PID)
    {
        var deletedObject = await context.TtStfreversalDetails.FindAsync(jobNo, PID)
            ?? throw new EntityDoesNotExistException();
        context.TtStfreversalDetails.Remove(deletedObject);
    }

    public int GetDetailsCount(string jobNo)
    {
        var count = context.TtStfreversalDetails
            .AsNoTracking()
            .Where(x => x.JobNo == jobNo)
            .Count();

        return count;
    }

    public bool DetailExists(string jobNo, string PID)
    {
        var exists = context.TtStfreversalDetails
            .AsNoTracking()
            .Where(x => x.JobNo == jobNo)
            .Where(x => x.Pid == PID)
            .Any();

        return exists;
    }

    public bool OutstandingReversalExistsForPID(string PID)
    {
        var exists = context.TtStfreversalMasters
            .AsNoTracking()
            .Join(
                context.TtStfreversalDetails.AsNoTracking(),
                stfr => stfr.JobNo,
                stfrd => stfrd.JobNo,
                (stfr, stfrd) => new { stfr.Status, stfrd.Pid })
            .Where(x => x.Pid == PID)
            .Where(x => x.Status != (byte)StockTransferReversalStatus.Completed)
            .Where(x => x.Status != (byte)StockTransferReversalStatus.Cancelled)
            .Any();

        return exists;
    }

    public async Task<StockTransferDetailInfo?> GetStockTransferDetailInfo(string jobNo, string PID)
    {
        var detail = await context.TtStockTransferDetails
            .Where(x => x.JobNo == jobNo)
            .Where(x => x.Pid == PID)
            .SingleOrDefaultAsync();
        var sd = await context.TtStorageDetails.FindAsync(PID);

        if (detail is null || sd is null)
            return null;

        return new StockTransferDetailInfo
        {
            LocationCode = detail.LocationCode,
            OriginalLocationCode = detail.OriginalLocationCode,
            OriginalSupplierID = detail.OriginalSupplierId,
            OriginalWHSCode = detail.OriginalWhscode,
            WHSCode = detail.Whscode,
            ProductCode = sd.ProductCode
        };
    }

    public async Task AddNewDetail(StockTransferReversalDetail newStockTransferReversalDetail)
    {
        var stockTransferReversalDetail = mapper.Map<TtStfReversalDetail>(newStockTransferReversalDetail);

        context.TtStfreversalDetails.Add(stockTransferReversalDetail);
        await context.SaveChangesAsync();
    }

    public List<ReversibleStockTransferItemDto> GetReversibleStockTransferItems(string stfJobNo, string? orderBy, bool orderByDescending)
    {
        var statusPutawayValue = StorageStatus.Putaway;
        var query = context.TtStockTransferDetails
            .AsNoTracking()
            .Join(
                context.TtStorageDetails,
                detail => detail.Pid,
                sd => sd.Pid,
                (detail, sd) => new { detail, sd })
            .Where(x => x.detail.JobNo == stfJobNo)
            .Where(x => x.sd.Status == statusPutawayValue)
            .Select(x => new ReversibleStockTransferItemDto
            {
                PID = x.detail.Pid,
                ProductCode = x.sd.ProductCode,
                Qty = (int)x.sd.Qty,
                LocationCode = x.detail.LocationCode,
                OriginalLocationCode = x.detail.OriginalLocationCode,
            });

        orderBy ??= "pid";
        var sorted = query
            .OrderByDescOrAsc(orderByDescending, (i) =>
                orderBy.ToLower() == "pid" ? i.PID :
                orderBy.ToLower() == "productcode" ? i.ProductCode :
                orderBy.ToLower() == "qty" ? i.Qty :
                orderBy.ToLower() == "location" ? i.LocationCode :
                orderBy.ToLower() == "originallocationcode" ? i.OriginalLocationCode :
                i.PID);

        var itemsInStockTransfer = sorted
            .ToList();

        var statusCancelledValue = (byte)StockTransferReversalStatus.Cancelled;
        var pidsInNotCancelledReversals = context.TtStfreversalMasters
            .AsNoTracking()
            .Where(master => master.Status != statusCancelledValue)
            .Join(
                context.TtStfreversalDetails.AsNoTracking(),
                master => master.JobNo,
                detail => detail.JobNo,
                (_, detail) => detail.Pid)
            .ToList();

        var items = itemsInStockTransfer
            .Where(x => !pidsInNotCancelledReversals.Contains(x.PID))
            .ToList();

        return items;
    }

    public List<StockTransferReversalDetail> GetStockTransferReversalDetails(string jobNo)
    {
        var items = context.TtStfreversalDetails
            .AsNoTracking()
            .Where(x => x.JobNo == jobNo)
            .OrderBy(x => x.ProductCode)
            .ProjectTo<StockTransferReversalDetail>(mapper.ConfigurationProvider)
            .ToList();

        return items;
    }

    public async Task<List<StockTransferReversalSummary>> GetStockTransferReversalSummary(string jobNo)
    {
        var items = await context.TtStfreversalDetails
            .AsNoTracking()
            .Join(
                context.TtStorageDetails.AsNoTracking(),
                ird => ird.Pid,
                sd => sd.Pid,
                (detail, sd) => new { detail, sd })
            .Where(x => x.detail.JobNo == jobNo)
            .GroupBy(x => new { x.detail.JobNo, x.sd.CustomerCode, x.sd.SupplierId, x.sd.ProductCode })
            .Select(g => new StockTransferReversalSummary
            {
                JobNo = g.Key.JobNo,
                ProductCode = g.Key.ProductCode,
                CustomerCode = g.Key.CustomerCode ?? "",
                SupplierId = g.Key.SupplierId,
                TotalQty = (int)g.Sum(x => x.sd.Qty),
                TotalPkg = g.Count(),
            })
            .ToListAsync();

        return items;
    }

    public string GetSupplierParadigm(string supplierId, string factoryId)
    {
        var supplier = context.SupplierMaster.Find(factoryId, supplierId)
            ?? throw new EntityDoesNotExistException();
        return supplier.SupplyParadigm;
    }

    private string FormatForWildcardSearch(string? value) 
        => value?.FormatForLikeExpr().Replace($"{EFCore.ESCAPE_CHAR}%", "%") ?? string.Empty;

}
