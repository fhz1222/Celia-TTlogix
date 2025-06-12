using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class InboundRepository : IInboundRepository
{
    private readonly AppDbContext dbContext;
    private readonly IMapper mapper;

    public InboundRepository(AppDbContext appDbContext, IMapper mapper)
    {
        this.dbContext = appDbContext;
        this.mapper = mapper;
    }

    public InboundStatus? GetStatus(string jobNo)
    {
        var query = dbContext.TtInbounds.Where(i => i.JobNo == jobNo).Select(i => i.Status).ToList();
        return query.Count == 0 ? null : InboundStatus.From(query.First());
    }

    public void UpdateStatusIfNoPutawayPallets(string jobNo)
    {
        var query =
            from i in dbContext.TtInbounds
            join s in dbContext.TtStorageDetails on i.JobNo equals s.InJobNo
            where i.JobNo == jobNo && s.Status != (byte)StorageStatus.Cancelled && s.LocationCode != string.Empty
            select s;

        var hasPutawayPallets = query.Any();
        if (!hasPutawayPallets)
        {
            dbContext.Database.ExecuteSqlRaw("UPDATE TT_Inbound SET Status = {0} WHERE JobNo = {1}", (byte)InboundStatus.New, jobNo);
        }
    }
}
