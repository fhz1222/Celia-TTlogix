using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.Decants.Commands.CompleteDecantCommand;

public class CompleteDecantCommand : IRequest
{
    public string JobNo { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class CompleteDecantCommandHandler : IRequestHandler<CompleteDecantCommand>
{
    private readonly IRepository repository;
    private readonly IDateTime dateTimeService;
    private readonly IPIDGenerator pidGenerator;
    private readonly IInventoryTransactionService inventoryTransactionService;

    public CompleteDecantCommandHandler(IRepository repository, IDateTime dateTimeService, IPIDGenerator pidGenerator, IInventoryTransactionService inventoryTransactionService)
    {
        this.repository = repository;
        this.dateTimeService = dateTimeService;
        this.pidGenerator = pidGenerator;
        this.inventoryTransactionService = inventoryTransactionService;
    }

    public async Task<Unit> Handle(CompleteDecantCommand request, CancellationToken cancellationToken)
    {
        repository.BeginTransaction();
        var decant = await repository.Decant.GetDecant(request.JobNo);
        try
        {
            if (decant == null)
                throw new UnknownJobNoException();
            if (!decant.CanEdit)
                throw new IllegalAdjustmentChangeException($"Decant with status 'Cancelled' or 'Completed' cannot be updated");

            var newPids = pidGenerator.GetNewPIDs(repository, decant.Items.SelectMany(i => i.NewPallets).Count());
            var newPidIndex = 0;
            foreach (var decantItem in decant.Items)
            {
                // STEP 1 - Update storage detail
                var pallet = repository.StorageDetails.GetPalletDetail(decantItem.SourcePalletId);
                if (pallet == null)
                    throw new UnknownPIDException();
                if (!pallet.Status.Equals(StorageStatus.Decant))
                    throw new IllegalDecantActionException($"Decant action cannot be completed - incorrect status of PID '{pallet.Id}'");

                pallet.Qty = 0;
                await repository.StorageDetails.Update(pallet);

                // STEP 2 - Create new pallets for each of decant item pallet and update decant item pallet with the new palletId
                foreach (var decantItemPallet in decantItem.NewPallets)
                {
                    var pidNo = newPids[newPidIndex++];
                    var newPallet = decantItemPallet.CreateNewPallet(pidNo, decant.CustomerCode);
                    await repository.StorageDetails.AddNewPallet(newPallet, pallet.Id);
                    decantItemPallet.PalletId = pidNo;
                }
                await repository.Decant.UpdateDecantItemOnComplete(decant.JobNo, decantItem);
            }
            //STEP 3 - add transactions
            await inventoryTransactionService.GenerateInventoryTransactionsOnDecantComplete(decant);

            // STEP 4 - Update decant (set status to complete)
            decant.Complete(request.UserCode, dateTimeService.Now);
            await repository.Decant.UpdateDecant(decant);

            await repository.SaveChangesAsync(cancellationToken);
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
