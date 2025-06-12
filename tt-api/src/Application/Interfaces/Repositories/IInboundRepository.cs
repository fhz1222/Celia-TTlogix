using Domain.ValueObjects;

namespace Application.Interfaces.Repositories;

public interface IInboundRepository
{
    InboundStatus? GetStatus(string jobNo);
    void UpdateStatusIfNoPutawayPallets(string jobNo);
}
