using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.Adjustments.Commands.AddNewAdjustmentCommand;

public class AddNewAdjustmentCommand : IRequest<string>
{
    public string CustomerCode { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public string UserCode { get; set; } = null!;
    public bool IsUndoZeroOut { get; set; }
}

public class AddNewAdjustmentCommandHandler : IRequestHandler<AddNewAdjustmentCommand, string>
{
    private readonly IJobNumberGenerator jobNumberGenerator;
    private readonly IRepository repository;

    public AddNewAdjustmentCommandHandler(IJobNumberGenerator jobNumberGenerator, IRepository repository
        )
    {
        this.jobNumberGenerator = jobNumberGenerator;
        this.repository = repository;
    }

    public async Task<string> Handle(AddNewAdjustmentCommand request, CancellationToken cancellationToken)
    {
        // check if warehouse code and customer code exists 
        if (!repository.Utils.CheckIfWhsCodeExists(request.WhsCode))
            throw new UnknownWhsCodeException();
        if (!repository.Utils.CheckIfCustomerCodeExists(request.CustomerCode))
            throw new UnknownCustomerCodeException();
        if (!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new UnknownUserCodeException();

        var jobNumber = jobNumberGenerator.GetJobNumber(repository.Adjustments);
        InventoryAdjustmentJobType jobType = request.IsUndoZeroOut ? InventoryAdjustmentJobType.UndoZeroOut : InventoryAdjustmentJobType.Normal;
        return await repository.Adjustments.AddNewAdjustment(request.WhsCode, request.CustomerCode, request.UserCode,
            jobType, InventoryAdjustmentStatus.New, jobNumber, cancellationToken);
    }
}
