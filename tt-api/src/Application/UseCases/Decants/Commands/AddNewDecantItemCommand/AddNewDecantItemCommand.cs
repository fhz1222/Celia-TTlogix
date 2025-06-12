using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.Decants.Commands.AddNewDecantItemCommand;

public class AddNewDecantItemCommand : IRequest
{
    public string JobNo { get; set; } = null!;
    public string UserCode { get; set; } = null!;
    public string PalletId { get; set; } = null!;
    public ICollection<int> NewQuantities { get; set; } = null!;
}

public class AddNewDecantItemCommandHandler : IRequestHandler<AddNewDecantItemCommand, Unit>
{
    private readonly IRepository repository;

    public AddNewDecantItemCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Unit> Handle(AddNewDecantItemCommand request, CancellationToken cancellationToken)
    {
        var decant = await repository.Decant.GetDecant(request.JobNo);
        if (decant == null)
            throw new UnknownJobNoException();

        if (!decant.CanEdit)
            throw new IllegalAdjustmentChangeException($"Decant with status 'Cancelled' or 'Completed' cannot be updated");

        repository.BeginTransaction();

        try
        {
            var pallet = repository.StorageDetails.GetPalletDetail(request.PalletId);
            if (pallet == null)
            {
                throw new UnknownPIDException();
            }

            if (!pallet.CanBeDecanted(decant.WhsCode, decant.CustomerCode))
                throw new IllegalDecantActionException($"Unable to decant PID {request.PalletId}");

            // check if sum(new quantities) = pallet.quantity
            if (request.NewQuantities.Sum() != pallet.Qty)
            {
                throw new IllegalDecantActionException($"Sum of the new quantities is not the same as old PID quantity");
            }
            // add new decant item and save to repository
            var newDecantItem = decant.AddDecantItem(pallet, request.NewQuantities);

            await repository.Decant.AddDecantItem(decant.JobNo, request.UserCode, newDecantItem);
            if (decant.Status.Equals(DecantStatus.New))
            {
                decant.Status = DecantStatus.Processing;
                await repository.Decant.UpdateDecant(decant);
            }
            // update pallet - status should be changed to Decant
            pallet.Status = StorageStatus.Decant;
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
