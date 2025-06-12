using Application.Exceptions;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.Customer.Commands.ToggleActiveUomDecimalCommand;

public class ToggleActiveUomDecimalCommand : IRequest<UomDecimal>
{
    public string Code { get; set; } = null!;
    public string CustomerCode { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class ToggleActiveUomDecimalCommandHandler : IRequestHandler<ToggleActiveUomDecimalCommand, UomDecimal>
{
    private readonly IRepository repository;

    public ToggleActiveUomDecimalCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<UomDecimal> Handle(ToggleActiveUomDecimalCommand request, CancellationToken cancellationToken)
    {
        if(!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new UnknownUserCodeException();

        var obj = repository.Customers.TryGetUomDecimal(request.CustomerCode, request.Code)
            ?? throw new ApplicationError($"Cannot find UOM decimal {request.CustomerCode} {request.Code}.");

        repository.BeginTransaction();
        try
        {
            if (obj.Status == UomDecimalStatus.Active)
            {
                obj.Status = UomDecimalStatus.Inactive;
                obj.CancelledBy = request.UserCode;
                obj.CancelledDate = DateTime.Now;
            }
            else
            {
                obj.Status = UomDecimalStatus.Active;
                obj.CancelledBy = "";
                obj.CancelledDate = null;
            }
            repository.Customers.UpdateUomDecimal(obj);

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
