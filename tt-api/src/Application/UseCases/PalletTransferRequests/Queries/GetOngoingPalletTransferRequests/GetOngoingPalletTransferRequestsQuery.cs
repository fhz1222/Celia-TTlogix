using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.PalletTransferRequests.Queries.GetOngoingPalletTransferRequests;

public class GetOngoingPalletTransferRequestsQuery : IRequest<IEnumerable<PalletTransferRequest>>
{
}

public class GetOngoingPalletTransferRequestsQueryHandler : IRequestHandler<GetOngoingPalletTransferRequestsQuery, IEnumerable<PalletTransferRequest>>
{
    private readonly IPalletTransferRequestsRepository repository;

    public GetOngoingPalletTransferRequestsQueryHandler(IPalletTransferRequestsRepository repository)
    {
        this.repository = repository;
    }

    public async Task<IEnumerable<PalletTransferRequest>> Handle(GetOngoingPalletTransferRequestsQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetOngoing();
    }
}
