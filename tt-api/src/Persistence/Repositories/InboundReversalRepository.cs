using Application.Common.Models;
using Application.Interfaces.Repositories;
using Application.UseCases.InboundReversalItems;
using Application.UseCases.InboundReversalItems.Commands.AddNewInboundReversalItemCommand;
using Application.UseCases.InboundReversalItems.Queries.GetInboundReversalItems;
using Application.UseCases.InboundReversalItems.Queries.GetReversibleInboundItems;
using Application.UseCases.InboundReversals;
using Application.UseCases.InboundReversals.Commands.CompleteInboundReversalCommand;
using Application.UseCases.InboundReversals.Queries.GetInboundReversals;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;
using EFCore = Persistence.EFCoreExtensions;
using InboundStatus = Domain.ValueObjects.InboundStatus;

namespace Persistence.Repositories;

public class InboundReversalRepository : IInboundReversalRepository
{
    private readonly AppDbContext context;
    private readonly AutoMapper.IMapper mapper;

    public CodePrefix GetCodePrefix => CodePrefix.InboundReversal;

    public int GetLastJobNumber(string prefix)
    {
        var jobNo = context.TtInboundReversals
            .AsNoTracking()
            .Where(x => EF.Functions.Like(x.JobNo, $"{prefix}%"))
            .OrderByDescending(x => x.JobNo)
            .Select(x => x.JobNo)
            .FirstOrDefault();

        var no = jobNo is null ? 0 : Convert.ToInt32(jobNo[^5..]);
        return no;
    }

    public InboundReversalRepository(AppDbContext context, AutoMapper.IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public PaginatedList<InboundReversalDto> GetInboundReversals(
        PaginationQuery pagination,
        GetInboundReversalsDtoFilter filter,
        string? orderBy,
        bool orderByDescending)
    {
        var query = context.TtInboundReversals
            .AsNoTracking()
            .OptionalWhere(filter.CreatedDate, createdDate => row => row.CreatedDate >= createdDate.From && row.CreatedDate <= createdDate.To);

        if (filter.Statuses != null && filter.Statuses.Any())
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
            string s => query.Where(ir => ir.WhsCode == s)
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
                orderBy.ToLower() == "whscode" ? i.WhsCode :
                orderBy.ToLower() == "customercode" ? i.CustomerCode :
                orderBy.ToLower() == "refno" ? i.RefNo :
                orderBy.ToLower() == "reason" ? i.Reason :
                orderBy.ToLower() == "createddate" ? i.CreatedDate :
                orderBy.ToLower() == "status" ? i.Status :
                i.JobNo);

        var items = querableResult
            .Skip((pagination.PageNumber - 1) * pagination.ItemsPerPage)
            .Take(pagination.ItemsPerPage)
            .ProjectTo<InboundReversalDto>(mapper.ConfigurationProvider)
            .ToList();

        var count = query.Count();

        return new PaginatedList<InboundReversalDto>(items, count, pagination.PageNumber, pagination.ItemsPerPage);
    }

    public PaginatedList<ReversibleInboundDto> GetReversibleInbounds(
        PaginationQuery pagination,
        string whsCode,
        string? inJobNo,
        DateTime? newerThan)
    {
        var completedStatusValue = (byte)InboundStatus.Completed;

        var query = context.TtInbounds
            .AsNoTracking()
            .Where(x => x.Whscode == whsCode)
            .Where(x => x.Status == completedStatusValue)
            .OptionalWhere(inJobNo, jobNo => row => row.JobNo == jobNo)
            .OptionalWhere(newerThan, date => row => row.CreatedDate >= date)
            .OrderByDescending(x => x.JobNo);

        var inbounds = query
            .Skip((pagination.PageNumber - 1) * pagination.ItemsPerPage)
            .Take(pagination.ItemsPerPage)
            .Select(x => new ReversibleInboundDto
            {
                JobNo = x.JobNo,
                RefNo = x.RefNo,
                SupplierId = x.SupplierId,
            })
            .ToList();

        var count = query.Count();

        return new PaginatedList<ReversibleInboundDto>(inbounds, count, pagination.PageNumber, pagination.ItemsPerPage);
    }

    public async Task AddNewInboundReversal(InboundReversal newInboundReversal)
    {
        var inboundReversal = mapper.Map<TtInboundReversal>(newInboundReversal);

        context.TtInboundReversals.Add(inboundReversal);
        await context.SaveChangesAsync();
    }

    public async Task<InboundInfo?> GetInboundInfo(string inJobNo)
    {
        var inbound = await context.TtInbounds.FindAsync(inJobNo);
        
        return inbound is null ? null : new InboundInfo
        {
            CustomerCode = inbound.CustomerCode,
            RefNo = inbound.RefNo,
            SupplierId = inbound.SupplierId,
            WhsCode = inbound.Whscode,
            Status = InboundStatus.From(inbound.Status),
            Type = InboundType.From(inbound.TransType),
        };
    }

    public async Task<InboundReversal?> GetInboundReversal(string jobNo)
    {
        var reversal = await context.TtInboundReversals.FindAsync(jobNo);
        return mapper.Map<InboundReversal>(reversal);
    }

    public async Task<bool> AnyInboundReversalDetailsExists(string jobNo)
    {
        var result = await context.TtInboundReversalDetails
            .Where(x =>  x.JobNo == jobNo)
            .AnyAsync();

        return result;
    }

    public async Task UpdateInboundReversal(InboundReversal updated)
    {
        var existing = await context.FindAsync<TtInboundReversal>(updated.JobNo)
            ?? throw new EntityDoesNotExistException();

        mapper.Map(updated, existing);
    }

    public async Task<string?> GetCustomerName(string customerCode, string whsCode)
    {
        var customer = await context.TtCustomers.FindAsync(customerCode, whsCode);
        return customer?.Name;
    }

    public async Task<string?> GetSupplierName(string supplierId, string factoryId)
    {
        var supplier = await context.SupplierMaster.FindAsync(factoryId, supplierId);
        return supplier?.CompanyName;
    }

    public IEnumerable<ReversibleInboundItemDto> GetReversibleInboundItems(
        string inJobNo,
        GetReversibleInboundItemsDtoFilter filter,
        string? orderBy,
        bool orderByDescending)
    {
        var statusPutawayValue = StorageStatus.Putaway;
        var query = context.TtStorageDetails
            .AsNoTracking()
            .Where(x => x.InJobNo == inJobNo)
            .Where(x => x.Qty != 0)
            .Where(x => x.Status == statusPutawayValue)
            .OptionalWhere(filter.PID, pid => row => row.Pid == pid)
            .OptionalWhere(filter.ProductCode, productCode => row => row.ProductCode == productCode)
            .OptionalWhere(filter.LocationCode, locationCode => row => row.LocationCode == locationCode)
            .Select(x => new ReversibleInboundItemDto
             {
                 PID = x.Pid,
                 ProductCode = x.ProductCode,
                 Qty = (int)x.Qty,
                 LocationCode = x.LocationCode,
             });

        orderBy ??= "pid";
        var sorted = query
            .OrderByDescOrAsc(orderByDescending, (i) =>
                orderBy.ToLower() == "pid" ? i.PID :
                orderBy.ToLower() == "productcode" ? i.ProductCode :
                orderBy.ToLower() == "qty" ? i.Qty :
                orderBy.ToLower() == "location" ? i.LocationCode :
                i.PID);

        var itemsInInbound = sorted
            .ToList();

        var statusCompletedValue = (byte)InboundReversalStatus.Completed;
        var pidsInNotCompletedReversals = context.TtInboundReversals
            .AsNoTracking()
            .Where(ir => ir.Status != statusCompletedValue)
            .Join(
                context.TtInboundReversalDetails.AsNoTracking(),
                ir => ir.JobNo,
                ird => ird.JobNo,
                (ir, ird) => ird.Pid)
            .ToList();

        var items = itemsInInbound
            .Where(x => !pidsInNotCompletedReversals.Contains(x.PID))
            .ToList();

        return items;
    }

    public bool OutstandingInboundReversalExistsForPID(string PID)
    {
        var statusCompletedValue = (byte)InboundReversalStatus.Completed;
        var statusCancelledValue = (byte)InboundReversalStatus.Cancelled;

        var exists = context.TtInboundReversals
            .AsNoTracking()
            .Join(
                context.TtInboundReversalDetails.AsNoTracking(),
                ir => ir.JobNo,
                ird => ird.JobNo,
                (ir, ird) => new { ir, ird.Pid })
            .Where(x => x.Pid == PID)
            .Where(x => x.ir.Status != statusCompletedValue)
            .Where(x => x.ir.Status != statusCancelledValue)
            .Any();

        return exists;
    }

    public bool InboundReversalDetailExists(string jobNo, string PID)
    {
        var exists = context.TtInboundReversalDetails
            .AsNoTracking()
            .Where(x => x.JobNo == jobNo)
            .Where(x => x.Pid == PID)
            .Any();

        return exists;
    }

    public async Task<PIDInfo?> GetPIDInfo(string PID)
    {
        var sd = await context.TtStorageDetails.FindAsync(PID);

        return sd is null ? null : new PIDInfo
        {
            OriginalQty = (int)sd.OriginalQty,
            Pid = sd.Pid,
            ProductCode = sd.ProductCode,
            InJobNo = sd.InJobNo,
        };
    }

    public async Task AddNewInboundReversalDetail(InboundReversalDetail newInboundReversalDetail)
    {
        var inboundReversalDetail = mapper.Map<TtInboundReversalDetail>(newInboundReversalDetail);

        context.TtInboundReversalDetails.Add(inboundReversalDetail);
        await context.SaveChangesAsync();
    }

    public async Task<InboundReversalDetail?> GetInboundReversalDetail(string jobNo, string PID)
    {
        var reversal = await context.TtInboundReversalDetails.FindAsync(jobNo, PID);
        return reversal is null ? null : mapper.Map<InboundReversalDetail>(reversal);
    }

    public async Task DeleteInboundReversalDetail(string jobNo, string PID)
    {
        var deletedObject = await context.TtInboundReversalDetails.FindAsync(jobNo, PID)
            ?? throw new EntityDoesNotExistException();
        context.TtInboundReversalDetails.Remove(deletedObject);
    }

    public int GetInboundReversalDetailsCount(string jobNo)
    {
        var count = context.TtInboundReversalDetails
            .AsNoTracking()
            .Where(x => x.JobNo == jobNo)
            .Count();

        return count;
    }

    public IEnumerable<InboundReversalItemDto> GetInboundReversalItems(
        string jobNo, GetInboundReversalItemsDtoFilter filter, string? orderBy, bool orderByDescending)
    {
        var query = context.TtInboundReversalDetails
            .AsNoTracking()
            .Where(x => x.JobNo == jobNo)
            .OptionalWhere(filter.PID, PID => row => row.Pid == PID)
            .OptionalWhere(filter.ProductCode, code => row => row.ProductCode == code);

        orderBy ??= "pid";
        query = query
            .OrderByDescOrAsc(orderByDescending, (i) =>
                orderBy.ToLower() == "pid" ? i.Pid :
                orderBy.ToLower() == "originalqty" ? i.OriginalQty :
                orderBy.ToLower() == "productcode" ? i.ProductCode :
                i.Pid);

        var items = query
            .Select(x => new InboundReversalItemDto
            {
                PID = x.Pid,
                OriginalQty = (int)x.OriginalQty,
                ProductCode = x.ProductCode,
            })
            .ToList();

        return items;
    }

    public List<InboundReversalDetail> GetInboundReversalDetails(string jobNo)
    {
        var items = context.TtInboundReversalDetails
            .AsNoTracking()
            .Where(x => x.JobNo == jobNo)
            .OrderBy(x => x.ProductCode)
            .ProjectTo<InboundReversalDetail>(mapper.ConfigurationProvider)
            .ToList();

        return items;
    }

    public async Task<List<InboundReversalSummary>> GetInboundReversalSummary(string jobNo)
    {
        var items = await context.TtInboundReversalDetails
            .AsNoTracking()
            .Join(
                context.TtStorageDetails.AsNoTracking(),
                ird => ird.Pid,
                sd => sd.Pid,
                (ird, sd) => new { ird, sd })
            .Where(x => x.ird.JobNo == jobNo)
            .GroupBy(x => new { x.ird.JobNo, x.ird.ProductCode, x.sd.CustomerCode, x.sd.SupplierId, x.sd.Ownership })
            .Select(g => new InboundReversalSummary
            {
                JobNo = g.Key.JobNo,
                ProductCode = g.Key.ProductCode,
                CustomerCode = g.Key.CustomerCode ?? "",
                SupplierId = g.Key.SupplierId,
                Ownership = Ownership.From(g.Key.Ownership),
                TotalDifferent = (int)g.Sum(x => x.sd.OriginalQty),
                TotalDiffPkg = g.Count(),
            })
            .ToListAsync();

        return items;
    }

    public async Task<List<InboundReversalSummaryByProduct>> GetInboundReversalSummaryByProduct(string jobNo)
    {
        var items = await context.TtInboundReversalDetails
            .AsNoTracking()
            .Join(
                context.TtStorageDetails.AsNoTracking(),
                ird => ird.Pid,
                sd => sd.Pid,
                (ird, sd) => new { ird, sd })
            .Where(x => x.ird.JobNo == jobNo)
            .GroupBy(x => new { x.ird.JobNo, x.ird.ProductCode, x.sd.CustomerCode })
            .Select(g => new InboundReversalSummaryByProduct
            {
                JobNo = g.Key.JobNo,
                ProductCode = g.Key.ProductCode,
                CustomerCode = g.Key.CustomerCode ?? "",
                TotalDifferent = (int)g.Sum(x => x.sd.OriginalQty),
                TotalDiffPkg = g.Count(),
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
