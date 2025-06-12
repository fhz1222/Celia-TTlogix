using Application.Interfaces.Repositories;
using Application.UseCases.ILogPickingRequests.Queries.GetPickingRequest;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.Repositories;

public class ILogPickingRequestRepository : IILogPickingRequestRepository
{
    private readonly AppDbContext context;
    private readonly IMapper mapper;

    public ILogPickingRequestRepository(AppDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public void AddItems(PickingRequestRevision rev, IEnumerable<PickingRequestRevisionItem> items)
    {
        int lineNo = 1;
        foreach (var item in items)
        {
            context.ILogPickingRequestRevisionItems.Add(new ILogPickingRequestRevisionItem
            {
                PickingRequestId = rev.PickingRequestId,
                PickingRequestRevision = rev.Revision,
                LineNo = lineNo++,
                Pid = item.PalletId,
                ProductCode = item.ProductCode,
                Qty = item.Qty,
                SupplierId = item.SupplierId
            });
        }
    }

    public PickingRequestRevision AddNewRequest(Outbound outbound, string userCode, DateTime createdDate)
    {
        var request = new ILogPickingRequest { OutboundJobNo = outbound.JobNo };
        context.ILogPickingRequests.Add(request);
        context.SaveChanges();

        var rev = context.ILogPickingRequestRevisions.Add(new ILogPickingRequestRevision
        {
            PickingRequestId = request.Id,
            Revision = 1,
            CreatedBy = userCode,
            CreatedOn = createdDate,
            ClosedOn = null,
        }).Entity;
        return new PickingRequestRevision
        {
            PickingRequestId = rev.PickingRequestId,
            OutboundJobNo = request.OutboundJobNo,
            Revision = rev.Revision,
            CreatedOn = rev.CreatedOn,
            CreatedBy = rev.CreatedBy,
            ClosedOn = rev.ClosedOn
        };
    }

    public void AddNewRevision(PickingRequestRevision newRev)
    {
        var dbObject = mapper.Map<ILogPickingRequestRevision>(newRev);
        context.ILogPickingRequestRevisions.Add(dbObject);
    }

    public PickingRequestRevision? GetOpenRequest(string outboundJobNo)
    {
        var ids = context.ILogPickingRequests.Where(x => x.OutboundJobNo == outboundJobNo).Select(x => x.Id).ToList();
        var rev = context.ILogPickingRequestRevisions.FirstOrDefault(x => ids.Contains(x.PickingRequestId) && x.ClosedOn == null);

        // to automap
        return rev == null ? null : new PickingRequestRevision
        {
            PickingRequestId = rev.PickingRequestId,
            OutboundJobNo = outboundJobNo,
            Revision = rev.Revision,
            ClosedOn = rev.ClosedOn,
            CreatedBy = rev.CreatedBy,
            CreatedOn = rev.CreatedOn
        };
    }

    public List<PickingRequestRevision> GetPickingRequestRevisionsNoTracking(List<string> outboundJobNos)
    {
        return context.ILogPickingRequests
            .AsNoTracking()
            .Where(x => outboundJobNos.Contains(x.OutboundJobNo))
            .Join(context.ILogPickingRequestRevisions, x => x.Id, x => x.PickingRequestId,
            (p, r) => new { p.OutboundJobNo, rev = r })
            .Select(x => new PickingRequestRevision
            {
                PickingRequestId = x.rev.PickingRequestId,
                OutboundJobNo = x.OutboundJobNo,
                Revision = x.rev.Revision,
                ClosedOn = x.rev.ClosedOn,
                CreatedBy = x.rev.CreatedBy,
                CreatedOn = x.rev.CreatedOn

            }).ToList();
    }

    public PickingRequestRevision? GetLastRevision(string requestId)
    {
        var request = context.ILogPickingRequestRevisions
            .Where(p => p.PickingRequestId == requestId)
            .OrderByDescending(p => p.Revision)
            .FirstOrDefault();
        return request == null ? null : MapToEntity(request);
    }

    public PickingRequestRevision? GetRevision(string requestId, int revision)
    {
        var request = context.ILogPickingRequestRevisions.Find(requestId, revision);
        return request == null ? null : MapToEntity(request);
    }

    public List<PickingRequestLineDto> GetItemsWithUnloadingPoint(string requestId, int revision)
    {
        var query =
            from i in context.ILogPickingRequestRevisionItems.AsNoTracking()
            where i.PickingRequestId == requestId && i.PickingRequestRevision == revision
            join r in context.ILogPickingRequests on i.PickingRequestId equals r.Id
            join o in context.TtOutbounds on r.OutboundJobNo equals o.JobNo
            join pm in context.TtPartMasters on new { o.CustomerCode, i.SupplierId, i.ProductCode } equals new { pm.CustomerCode, pm.SupplierId, ProductCode = pm.ProductCode1 }
            join up in context.TtUnloadingPoints on pm.UnloadingPointId equals up.Id into upg
            from result in upg.DefaultIfEmpty()
            select new PickingRequestLineDto 
            {
                LineNo = i.LineNo,
                ProductCode = i.ProductCode,
                SupplierId = i.SupplierId,
                Qty = i.Qty,
                PalletId = i.Pid,
                ProductUnloadingPoint = result == null ? null : result.Name
            };
        return query.ToList();
    }

    public void Update(PickingRequestRevision revision)
    {
        var rev = context.ILogPickingRequestRevisions.Find(revision.PickingRequestId, revision.Revision)
            ?? throw new EntityDoesNotExistException();
        rev.ClosedOn = revision.ClosedOn;
    }

    public void CancelAllOpenRequests()
    {
        context.Database.ExecuteSqlRaw("UPDATE dbi.iLogPickingRequestRevision SET ClosedOn = '1900-01-01' WHERE ClosedOn IS NULL");
    }

    private PickingRequestRevision MapToEntity(ILogPickingRequestRevision request)
    {
        var obj = mapper.Map<PickingRequestRevision>(request);
        obj.OutboundJobNo = context.ILogPickingRequests.Find(request.PickingRequestId)?.OutboundJobNo!;
        return obj;
    }
}
