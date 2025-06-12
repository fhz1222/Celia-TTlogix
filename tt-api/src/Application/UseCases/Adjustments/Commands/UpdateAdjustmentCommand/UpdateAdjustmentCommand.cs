using Application.Exceptions;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Adjustments.Commands.UpdateAdjustmentCommand;

public class UpdateAdjustmentCommand : IRequest<Adjustment>
{
    public UpdatedAdjustmentVM Adjustment { get; set; }
}

public class UpdateAdjustmentCommandHandler : IRequestHandler<UpdateAdjustmentCommand, Adjustment>
{
    private readonly IRepository repository;

    public UpdateAdjustmentCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Adjustment> Handle(UpdateAdjustmentCommand request, CancellationToken cancellationToken)
    {
        repository.BeginTransaction();
        var adjustment = repository.Adjustments.GetAdjustmentDetails(request.Adjustment.JobNo);
        try
        {
            if (adjustment == null)
                throw new UnknownJobNoException();

            if (!adjustment.CanEdit)
                throw new IllegalAdjustmentChangeException($"Adjustment with status 'Cancelled' or 'Completed' cannot be updated");

            if (string.IsNullOrWhiteSpace(request.Adjustment.ReferenceNo))
            {
                throw new RequiredFieldException($"Adjustment requires Reference No field");
            }
            adjustment.ReferenceNo = request.Adjustment.ReferenceNo;
            adjustment.Reason = request.Adjustment.Reason;
            await repository.Adjustments.Update(adjustment);
            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();
            return adjustment;
        }
        catch (Exception)
        {
            repository.RollbackTransaction();
            throw;
        }
    }
}
