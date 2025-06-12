using Application.Exceptions;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.Customer.Commands.ToggleActiveCustomerClientCommand;

public class ToggleActiveCustomerClientCommand : IRequest<CustomerClient>
{
    public string Code { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class ToggleActiveCustomerClientCommandHandler : IRequestHandler<ToggleActiveCustomerClientCommand, CustomerClient>
{
    private readonly IRepository repository;

    public ToggleActiveCustomerClientCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<CustomerClient> Handle(ToggleActiveCustomerClientCommand request, CancellationToken cancellationToken)
    {
        if(!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new UnknownUserCodeException();

        var obj = repository.Customers.TryGetCustomerClient(request.Code)
            ?? throw new ApplicationError($"Cannot find customer client {request.Code}.");

        repository.BeginTransaction();
        try
        {
            if (obj.Status == CustomerClientStatus.Active)
            {
                obj.Status = CustomerClientStatus.Inactive;
                obj.CancelledBy = request.UserCode;
                obj.CancelledDate = DateTime.Now;
            }
            else
            {
                obj.Status = CustomerClientStatus.Active;
                obj.CancelledBy = "";
                obj.CancelledDate = null;
            }
            repository.Customers.UpdateCustomerClient(obj);

            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();
            return obj;
        }
        catch(Exception)
        {
            repository.RollbackTransaction();
            throw;
        }
    }
}
