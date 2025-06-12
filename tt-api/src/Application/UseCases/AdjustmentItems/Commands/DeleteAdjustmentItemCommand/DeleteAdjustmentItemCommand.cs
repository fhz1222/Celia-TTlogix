using Application.Exceptions;
using Application.Interfaces.Repositories;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.AdjustmentItems.Commands.DeleteAdjustmentItemCommand;

public class DeleteAdjustmentItemCommand : IRequest
{
    public string JobNo { get; set; }
    public int LineItem { get; set; }
}

public class DeleteAdjustmentItemCommandHandler : IRequestHandler<DeleteAdjustmentItemCommand>
{
    private readonly IRepository repository;

    public DeleteAdjustmentItemCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Unit> Handle(DeleteAdjustmentItemCommand request, CancellationToken cancellationToken)
    {
        repository.BeginTransaction();
        try
        {
            var adjustment = repository.Adjustments.GetAdjustmentDetails(request.JobNo);
            if (adjustment == null)
                throw new UnknownJobNoException();

            if (!adjustment.CanEdit)
                throw new IllegalAdjustmentChangeException("Item cannot be deleted because the adjustment is not editable");

            await repository.AdjustmentItems.Delete(request.JobNo, request.LineItem);

            // check if the status should be set against to 'New' (adjustment without items)
            var items = repository.AdjustmentItems.GetAdjustmentItems(request.JobNo, null, false).Where(i => i.LineItem != request.LineItem);
            if (items.Count() == 0)
            {
                adjustment.Status = InventoryAdjustmentStatus.New;
                await repository.Adjustments.Update(adjustment);
            }
            await repository.SaveChangesAsync();
            repository.CommitTransaction();
        }
        catch (Exception)
        {
            repository.RollbackTransaction();
            throw;
        }
        return await Task.FromResult(Unit.Value);
    }
}
