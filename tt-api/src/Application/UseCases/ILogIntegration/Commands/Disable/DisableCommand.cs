using Application.Exceptions;
using Application.Interfaces.Gateways;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.ILogIntegration.Commands.Disable;

public class DisableCommand : IRequest
{
}

public class DisableCommandHandler : IRequestHandler<DisableCommand>
{
    private readonly IRepository repository;
    private readonly IILogConnectGateway ilogConnectGateway;

    public DisableCommandHandler(IRepository repository, IILogConnectGateway ilogConnectGateway)
    {
        this.repository = repository;
        this.ilogConnectGateway = ilogConnectGateway;
    }

    public async Task<Unit> Handle(DisableCommand request, CancellationToken cancellationToken)
    {
        repository.BeginTransaction();

        try
        {
            var isEnabled = repository.ILogIntegrationRepository.GetStatus();
            if (!isEnabled)
            {
                throw new ApplicationError("Integration is not enabled.");
            }

            repository.Locations.ResetLocationCategories();
            repository.ILogBoxes.DeleteAllBoxes();
            repository.ILogPickingRequests.CancelAllOpenRequests();

            var openPtrs = await repository.PalletTransferRequests.GetOngoing();
            foreach (var ptr in openPtrs)
            {
                ptr.Cancel();
                await repository.PalletTransferRequests.Update(ptr);

                var pallet = repository.StorageDetails.GetPalletDetail(ptr.PID) ?? throw new UnknownPIDException();
                pallet.Unrestrict();
                await repository.StorageDetails.Update(pallet);
            }
            await repository.SaveChangesAsync(cancellationToken);

            repository.ILogIntegrationRepository.Disable();

            repository.CommitTransaction();
        }
        catch (Exception)
        {
            repository.RollbackTransaction();
            throw;
        }

        ilogConnectGateway.IntegrationStatusChanged();

        return await Task.FromResult(Unit.Value);
    }
}
