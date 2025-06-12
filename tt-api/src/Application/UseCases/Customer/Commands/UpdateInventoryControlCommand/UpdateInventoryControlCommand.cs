using Application.Exceptions;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Customer.Commands.UpdateInventoryControlCommand;

public class UpdateInventoryControlCommand : IRequest<InventoryControl>
{
    public InventoryControlDto InventoryControl { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class UpdateInventoryControlCommandHandler : IRequestHandler<UpdateInventoryControlCommand, InventoryControl>
{
    private readonly IRepository repository;

    public UpdateInventoryControlCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<InventoryControl> Handle(UpdateInventoryControlCommand request, CancellationToken cancellationToken)
    {
        if(!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new UnknownUserCodeException();

        var validSelectedControlCodes = new List<string>{
            "",
            "CC1TYPE",
            "CC2TYPE",
            "CC3TYPE",
            "CC4TYPE",
            "CC5TYPE",
            "CC6TYPE",
        };

        if(!validSelectedControlCodes.Contains(request.InventoryControl.SelectControlCode))
            throw new ApplicationError("Invalid selected control code.");

        repository.BeginTransaction();
        try
        {
            var obj = repository.Customers.TryGetInventoryControl<InventoryControl>(request.InventoryControl.CustomerCode);

            if (obj is null)
            {
                obj = new InventoryControl
                {
                    CustomerCode = request.UserCode,
                    Pc1type = request.InventoryControl.Pc1type,
                    Pc2type = request.InventoryControl.Pc2type,
                    Pc3type = request.InventoryControl.Pc3type,
                    Pc4type = request.InventoryControl.Pc4type,
                    Cc1type = request.InventoryControl.Cc1type,
                    Cc2type = request.InventoryControl.Cc2type,
                    Cc3type = request.InventoryControl.Cc3type,
                    Cc4type = request.InventoryControl.Cc4type,
                    Cc5type = request.InventoryControl.Cc5type,
                    Cc6type = request.InventoryControl.Cc6type,
                    SelectControlCode = request.InventoryControl.SelectControlCode,
                    RevisedBy = "",
                    RevisedDate = null,
                };

                repository.Customers.AddNewInventoryControl(obj);
            }
            else
            {
                obj.Pc1type = request.InventoryControl.Pc1type;
                obj.Pc2type = request.InventoryControl.Pc2type;
                obj.Pc3type = request.InventoryControl.Pc3type;
                obj.Pc4type = request.InventoryControl.Pc4type;
                obj.Cc1type = request.InventoryControl.Cc1type;
                obj.Cc2type = request.InventoryControl.Cc2type;
                obj.Cc3type = request.InventoryControl.Cc3type;
                obj.Cc4type = request.InventoryControl.Cc4type;
                obj.Cc5type = request.InventoryControl.Cc5type;
                obj.Cc6type = request.InventoryControl.Cc6type;
                obj.SelectControlCode = request.InventoryControl.SelectControlCode;
                obj.RevisedBy = request.UserCode;
                obj.RevisedDate = DateTime.Now;

                repository.Customers.UpdateInventoryControl(obj);
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
