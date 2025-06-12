using Application.Exceptions;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.Customer.Commands.UpdateUomDecimalCommand;

public class UpdateUomDecimalCommand : IRequest<UomDecimal>
{
    public UomDecimalDto UomDecimal { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class UpdateUomDecimalCommandHandler : IRequestHandler<UpdateUomDecimalCommand, UomDecimal>
{
    private readonly IRepository repository;

    public UpdateUomDecimalCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<UomDecimal> Handle(UpdateUomDecimalCommand request, CancellationToken cancellationToken)
    {
        if(!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new UnknownUserCodeException();

        repository.BeginTransaction();
        try
        {
            if(!repository.Customers.UomExists(request.UomDecimal.Uom))
                throw new ApplicationError($"UOM {request.UomDecimal.Uom} cannot be found.");

            var obj = repository.Customers.TryGetUomDecimal(request.UomDecimal.CustomerCode, request.UomDecimal.Uom);

            if (obj is null)
            {
                obj = new UomDecimal
                {
                    CustomerCode = request.UomDecimal.CustomerCode,
                    UOM = request.UomDecimal.Uom,
                    DecimalNum = request.UomDecimal.DecimalNum,
                    Status = UomDecimalStatus.Active,
                    CreatedBy = request.UserCode,
                    CreatedDate = DateTime.Now,
                    CancelledBy = "",
                    CancelledDate = null,
                };

                repository.Customers.AddNewUomDecimal(obj);
            }
            else
            {
                obj.DecimalNum = request.UomDecimal.DecimalNum;

                repository.Customers.UpdateUomDecimal(obj);
            }

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
