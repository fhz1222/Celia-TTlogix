using Application.Exceptions;
using Application.Extensions;
using Application.Interfaces.Gateways;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.ILogIntegration.Commands.Enable;

public class EnableCommand : IRequest
{
    public string WHSCode { get; set; }
}

public class EnableCommandHandler : IRequestHandler<EnableCommand>
{
    private readonly IRepository repository;
    private readonly IILogConnectGateway ilogConnectGateway;

    public EnableCommandHandler(IRepository repository, IILogConnectGateway ilogConnectGateway)
    {
        this.repository = repository;
        this.ilogConnectGateway = ilogConnectGateway;
    }

    public async Task<Unit> Handle(EnableCommand request, CancellationToken cancellationToken)
    {
        repository.BeginTransaction();

        try
        {
            var isEnabled = repository.ILogIntegrationRepository.GetStatus();
            if (isEnabled)
            {
                throw new ApplicationError("Integration is already enabled.");
            }

            var integratedWarehouses = repository.ILogIntegrationRepository.GetWarehouses();
            if (integratedWarehouses.DoesNotContain(request.WHSCode))
            {
                throw new ApplicationError($"Integration can't be enabled for warehouse {request.WHSCode}.");
            }

            integratedWarehouses.ForEach(wh => repository.Locations.RestoreILogSystemLocations(wh));
            await repository.SaveChangesAsync(cancellationToken);

            repository.ILogIntegrationRepository.Enable();

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
