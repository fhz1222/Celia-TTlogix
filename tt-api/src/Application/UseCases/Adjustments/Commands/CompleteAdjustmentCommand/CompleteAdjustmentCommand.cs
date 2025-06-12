using Application.Exceptions;
using Application.Interfaces.Gateways;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.Adjustments.Commands.CompleteAdjustmentCommand;

public class CompleteAdjustmentCommand : IRequest<Adjustment>
{
    public string JobNo { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class CompleteAdjustmentCommandHandler : IRequestHandler<CompleteAdjustmentCommand, Adjustment>
{
    private readonly IRepository repository;
    private readonly IDateTime dateTimeService;
    private readonly IInventoryTransactionService inventoryTransactionService;
    private readonly IILogConnectGateway ilogConnectGateway;

    public CompleteAdjustmentCommandHandler(IRepository repository, IDateTime dateTimeService, IInventoryTransactionService inventoryTransactionService, IILogConnectGateway ilogConnectGateway)
    {
        this.repository = repository;
        this.dateTimeService = dateTimeService;
        this.inventoryTransactionService = inventoryTransactionService;
        this.ilogConnectGateway = ilogConnectGateway;
    }

    public async Task<Adjustment> Handle(CompleteAdjustmentCommand request, CancellationToken cancellationToken)
    {
        // check if user code exists 
        if (!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new UnknownUserCodeException();

        repository.BeginTransaction();
        // check if adjustment exists and can confirm
        var adjustment = repository.Adjustments.GetAdjustmentDetails(request.JobNo);

        try
        {
            if (adjustment == null)
                throw new UnknownJobNoException();

            if (!adjustment.CanComplete)
                throw new IllegalAdjustmentChangeException($"Adjustment status cannot be changed from value 'Cancelled' to 'Completed' nor set the status to 'Completed' again'");

            //Perform Update StorageDetail and Quarantine Qty
            var items = repository.AdjustmentItems.GetAdjustmentItems(adjustment.JobNo, null, false);
            foreach (var item in items)
            {
                var pallet = repository.StorageDetails.GetPalletDetail(item.PID);
                if (pallet == null)
                    throw new UnknownPIDException();

                if (!pallet.CanBeAdjusted(adjustment.WhsCode, adjustment.CustomerCode, adjustment.JobType))
                    throw new AdjustmentFailException();

                if (pallet.Status.Equals(StorageStatus.Quarantine))
                {
                    var inventory = repository.Inventory.GetInventoryItem(pallet.WhsCode, pallet.Product.CustomerSupplier.CustomerCode, pallet.Product.CustomerSupplier.SupplierId, item.ProductCode, pallet.Ownership);
                    if (inventory == null)
                        throw new InventoryNotFoundException();

                    inventory.SetQuarantineValues(item.NewQty, item.InitialQty);

                    // update inventory (quarantine)
                    await repository.Inventory.Update(inventory);
                }

                pallet.Qty = item.NewQty;
                pallet.QtyPerPkg = item.NewQtyPerPkg;

                if (item.NewQty == 0)
                    pallet.Status = StorageStatus.ZeroOut;

                if (pallet.Status.Equals(StorageStatus.ZeroOut) && item.NewQty > 0 && item.InitialQty == 0)
                    pallet.Status = StorageStatus.Putaway;
                //update pallet
                await repository.StorageDetails.Update(pallet);
            }

            // Get inventory adjustment grouped data, then per each line get inventory and update
            var itemSummaries = repository.AdjustmentItems.GetAdjustmentItemGroupedData(adjustment.JobNo);
            foreach (var itemSummary in itemSummaries)
            {
                var inventory = repository.Inventory.GetInventoryItem(adjustment.WhsCode, itemSummary.CustomerCode, itemSummary.SupplierId, itemSummary.ProductCode, itemSummary.Ownership);

                if (inventory == null)
                    throw new InventoryNotFoundException();

                inventory.SetOnHandValues(itemSummary.TotalDifferent, itemSummary.TotalDifferentPkg);
                // update inventory
                await repository.Inventory.Update(inventory);
            }

            // set adjustment status to complete
            adjustment.Complete(request.UserCode, dateTimeService.Now);
            await repository.Adjustments.Update(adjustment);

            // add inventory transaction logs
            await inventoryTransactionService.GenerateInventoryTransactionsOnAdjustmentComplete(adjustment.JobNo);

            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();

            ilogConnectGateway.AdjustmentCompleted(adjustment.JobNo);
            
            return adjustment;
        }
        catch (Exception)
        {
            repository.RollbackTransaction();
            throw;
        }
    }
}
