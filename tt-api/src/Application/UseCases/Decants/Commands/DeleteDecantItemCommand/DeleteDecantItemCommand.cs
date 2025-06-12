using Application.Exceptions;
using Application.Interfaces.Repositories;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.Decants.Commands.DeleteDecantItemCommand;

public class DeleteDecantItemCommand : IRequest
{
    public string JobNo { get; set; } = null!;
    public string PalletId { get; set; } = null!;
}

public class DeleteDecantItemCommandHandler : IRequestHandler<DeleteDecantItemCommand, Unit>
{
    private readonly IRepository repository;

    public DeleteDecantItemCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Unit> Handle(DeleteDecantItemCommand request, CancellationToken cancellationToken)
    {
        var decant = await repository.Decant.GetDecant(request.JobNo);
        if (decant == null)
            throw new UnknownJobNoException();

        if (!decant.CanEdit)
            throw new IllegalAdjustmentChangeException($"Decant with status 'Cancelled' or 'Completed' cannot be updated");

        repository.BeginTransaction();

        try
        {
            var item = decant.Items.Where(i => i.SourcePalletId == request.PalletId).FirstOrDefault();
            if (item == null)
                throw new UnknownPIDException($"PID '{request.PalletId}' not found in decant '{request.JobNo}'");

            var pallet = repository.StorageDetails.GetPalletDetail(request.PalletId);
            if (pallet == null)
            {
                throw new UnknownPIDException();
            }
            decant.Items.Remove(item);

            await repository.Decant.DeleteDecantItem(decant.JobNo, item);
            if (decant.Items.Count == 0)
            {
                decant.Status = DecantStatus.New;
                await repository.Decant.UpdateDecant(decant);
            }
            // update pallet - status should be changed again to Putaway
            pallet.Status = StorageStatus.Putaway;
            await repository.StorageDetails.Update(pallet);

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
