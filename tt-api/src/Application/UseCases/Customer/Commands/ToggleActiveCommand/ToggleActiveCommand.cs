using Application.Exceptions;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Customer.Commands.ToggleActiveCommand;

public class ToggleActiveCommand : IRequest<Domain.Entities.Customer>
{
    public string Code { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class ToggleActiveCommandHandler : IRequestHandler<ToggleActiveCommand, Domain.Entities.Customer>
{
    private readonly IRepository repository;

    public ToggleActiveCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Domain.Entities.Customer> Handle(ToggleActiveCommand request, CancellationToken cancellationToken)
    {
        if(!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new UnknownUserCodeException();

        var obj = repository.Customers.TryGetCustomer(request.Code, request.WhsCode)
            ?? throw new ApplicationError($"Cannot find customer {request.Code} {request.WhsCode}.");

        repository.BeginTransaction();
        try
        {
            if (obj.Status == Domain.Enums.CustomerStatus.Active)
            {
                obj.Status = Domain.Enums.CustomerStatus.Inactive;
                obj.CancelledBy = request.UserCode;
                obj.CancelledDate = DateTime.Now;
            }
            else
            {
                obj.Status = Domain.Enums.CustomerStatus.Active;
                obj.CancelledBy = "";
                obj.CancelledDate = null;
            }
            repository.Customers.Update(obj);

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
