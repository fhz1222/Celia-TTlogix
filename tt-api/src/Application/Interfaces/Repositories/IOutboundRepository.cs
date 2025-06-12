using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IOutboundRepository
{
    Loading? GetLoadingByOutboundNoTracking(string outboundJob);
    List<Outbound> GetOutboundsOnLoading(string loadingJobNo);
}
