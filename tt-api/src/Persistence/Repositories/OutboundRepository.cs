using Application.Interfaces.Repositories;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class OutboundRepository : IOutboundRepository
{
    private readonly AppDbContext context;
    private readonly IMapper mapper;

    public OutboundRepository(AppDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public List<Outbound> GetOutboundsOnLoading(string loadingJobNo)
    {
        var jobNos = context.TtLoadingDetails.AsNoTracking().Where(x => x.JobNo == loadingJobNo).Select(x => x.OutJobNo).ToList();
        var outbounds = context.TtOutbounds.AsNoTracking().Where(x => jobNos.Contains(x.JobNo)).ProjectTo<Outbound>(mapper.ConfigurationProvider).ToList();
        return outbounds;
    }

    public Loading? GetLoadingByOutboundNoTracking(string outboundJob)
    {
        return context.TtLoadingDetails
            .AsNoTracking()
            .Where(ld => ld.OutJobNo == outboundJob)
            .Join(context.TtLoadings, d => d.JobNo, h => h.JobNo, (detail, header) => header)
            .ProjectTo<Loading>(mapper.ConfigurationProvider)
            .FirstOrDefault();
    }
}
