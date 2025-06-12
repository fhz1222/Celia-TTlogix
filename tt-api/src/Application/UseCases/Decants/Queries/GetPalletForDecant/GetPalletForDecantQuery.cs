using Application.Exceptions;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Decants.Queries.GetPalletForDecant;

public class GetPalletForDecantQuery : IRequest<Pallet>
{
    public string PID;
    public string JobNo;
}

public class GetPalletForDecantQueryHandler : IRequestHandler<GetPalletForDecantQuery, Pallet>
{
    private readonly IRepository repository;

    public GetPalletForDecantQueryHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Pallet> Handle(GetPalletForDecantQuery request, CancellationToken cancellationToken)
    {
        var decant = await repository.Decant.GetDecant(request.JobNo) ?? throw new UnknownJobNoException();
        var pallet = repository.StorageDetails.GetPalletDetail(request.PID) ?? throw new UnknownPIDException();
        if (!pallet.CanBeDecanted(decant.WhsCode, decant.CustomerCode))
            throw new IllegalDecantActionException($"Unable to decant PID {pallet.Id}");
        if (repository.Locations.IsILogInboundLocation(pallet.Location, pallet.WhsCode))
            throw new IllegalDecantActionException($"Cannot decant PID on ILog Inbound location.");
        if (repository.Locations.IsILogStorageLocation(pallet.Location, pallet.WhsCode))
            throw new IllegalDecantActionException($"Cannot decant PID on ILog Storage location.");
        return pallet;
    }
}

