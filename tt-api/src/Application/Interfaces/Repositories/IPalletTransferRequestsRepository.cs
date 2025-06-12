using Application.Interfaces.Utils;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IPalletTransferRequestsRepository : IJobNumberSource
{
    void Add(PalletTransferRequest palletTransferRequest);
    Task<PalletTransferRequest?> Get(string jobNo);
    Task Update(PalletTransferRequest ptr);
    Task <IEnumerable<PalletTransferRequest>> GetOngoing();
}
