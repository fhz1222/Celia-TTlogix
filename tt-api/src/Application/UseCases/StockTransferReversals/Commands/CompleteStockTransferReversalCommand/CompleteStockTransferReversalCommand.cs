using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.StockTransferReversals.Commands.CompleteStockTransferReversalCommand;

public class CompleteStockTransferReversalCommand : IRequest<StockTransferReversal>
{
    public string JobNo { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class CompleteStockTransferReversalCommandHandler : IRequestHandler<CompleteStockTransferReversalCommand, StockTransferReversal>
{
    private readonly IRepository repository;
    private IDateTime dateTimeService;
    private IAppSettings appSettings;

    public CompleteStockTransferReversalCommandHandler(IRepository repository, IDateTime dateTimeService, IAppSettings appSettings)
    {
        this.repository = repository;
        this.dateTimeService = dateTimeService;
        this.appSettings = appSettings;
    }

    public async Task<StockTransferReversal> Handle(CompleteStockTransferReversalCommand request, CancellationToken cancellationToken)
    {
        if(!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new ApplicationError($"User {request.UserCode} does not exist.");

        repository.BeginTransaction();
        try
        {
            // Step 1 : Get StockTransfer Reversal Master
            var stockTransferReversal = await repository.StockTransferReversals.GetStockTransferReversal(request.JobNo)
                ?? throw new ApplicationError($"Unknown StockTransfer reversal JobNo {request.JobNo}.");

            if(stockTransferReversal.Status == StockTransferReversalStatus.New)
                throw new ApplicationError($"Cannot complete empty stock transfer reversal.");

            if(stockTransferReversal.Status == StockTransferReversalStatus.Completed
                || stockTransferReversal.Status == StockTransferReversalStatus.Cancelled)
                throw new ApplicationError($"Cannot modify completed or cancelled stock transfer reversal.");

            // Step 2 : Get StockTransfer Reversal Detail
            var details = repository.StockTransferReversals.GetStockTransferReversalDetails(stockTransferReversal.JobNo);
            
            // Step 3 : Perform Update StorageDetail
            foreach(var detail in details)
            {
                // Step 3.1 : Get StockTransfer Reversal Detail Instance
                // Step 3.2 : Get Storage Detail Instance
                var pallet = repository.StorageDetails.GetPalletDetail(detail.Pid)
                    ?? throw new ApplicationError($"Unknown PID {detail.Pid}.");

                // Step 3.4 : Verify Record
                if(pallet.WhsCode != stockTransferReversal.WhsCode)
                    throw new ApplicationError($"Invalid Warehouse Code ({pallet.WhsCode}), Unable to reversal {detail.JobNo}-{detail.ProductCode}-{detail.Pid}");

                if(pallet.Status != StorageStatus.Putaway)
                    throw new ApplicationError($"{pallet.Status} item, Unable to reversal {detail.JobNo}({detail.Pid})-{detail.ProductCode}");

                // Step 3.5 : Update Storage Detail
                pallet.WhsCode = detail.Whscode;
                pallet.Location = detail.LocationCode;
                pallet.Status = StorageStatus.Putaway;
                pallet.Ownership = Ownership.Supplier;
                pallet.IsVmi = true;

                await repository.StorageDetails.Update(pallet);
            }

            await repository.SaveChangesAsync(cancellationToken);

            // Step 4.1 : Get StockTransfer Reversal Detail
            var summaries = await repository.StockTransferReversals.GetStockTransferReversalSummary(stockTransferReversal.JobNo);

            foreach (var summary in summaries)
            {
                // Step 4.2 : Adjust Inventory Detail (Sales Of Aged Stock)
                var invItemSales = repository.Inventory.GetInventoryItem(stockTransferReversal.WhsCode, summary.CustomerCode, summary.SupplierId, summary.ProductCode, Ownership.Supplier)
                    ?? throw new ApplicationError($"Fail to retrive Inventory Instance for {summary.JobNo}-{summary.ProductCode}");

                invItemSales.OnHandQty += summary.TotalQty;
                invItemSales.OnHandPkg += summary.TotalPkg;

                await repository.Inventory.Update(invItemSales);

                // Step 4.3 : Adjust Inventory Detail (Purchase Of Aged Stock)
                var invItemPurchase = repository.Inventory.GetInventoryItem(stockTransferReversal.WhsCode, summary.CustomerCode, summary.SupplierId, summary.ProductCode, Ownership.EHP)
                    ?? throw new ApplicationError($"Fail to retrive Inventory Instance for {summary.JobNo}-{summary.ProductCode}");

                invItemPurchase.OnHandQty -= summary.TotalQty;
                invItemPurchase.OnHandPkg -= summary.TotalPkg;

                await repository.Inventory.Update(invItemPurchase);

                // step 4.4 : Insert into InvTransactionPerSupplier (Sales Of Aged Stock)
                var lastInvTransSales = repository.InventoryTransactions.GetLatestTransactionPerSupplier(summary.CustomerCode, summary.ProductCode, summary.SupplierId, Ownership.Supplier)
                    ?? throw new ApplicationError($"No Last Inventory Transaction was found for {summary.JobNo}-{summary.ProductCode}");

                var newInvTransSales = new InventoryTransactionPerSupplier
                {
                    JobNo = stockTransferReversal.JobNo,
                    ProductCode = summary.ProductCode,
                    SupplierId = summary.SupplierId,
                    CustomerCode = summary.CustomerCode,
                    Ownership = Ownership.Supplier,
                    JobDate = stockTransferReversal.CreatedDate,
                    Act = InventoryTransactionType.ReversalOfSaleOfAgeStock,
                    Qty = Math.Abs(summary.TotalQty),
                    BalanceQty = lastInvTransSales.BalanceQty + summary.TotalQty,
                };

                if(newInvTransSales.BalanceQty < 0)
                    throw new ApplicationError($"Reversal fail as Balance Qty of {summary.ProductCode} in Inventory Warehouse Transaction by supplier< 0");

                repository.InventoryTransactions.AddTransaction(newInvTransSales);

                // step 4.5 : Insert into InvTransactionPerSupplier (Purchase Of Aged Stock)
                var lastInvTransPurchase = repository.InventoryTransactions.GetLatestTransactionPerSupplier(summary.CustomerCode, summary.ProductCode, summary.SupplierId, Ownership.EHP)
                    ?? throw new ApplicationError($"No Last Inventory Transaction was found for {summary.JobNo}-{summary.ProductCode}");

                var newInvTransPurchase = new InventoryTransactionPerSupplier
                {
                    JobNo = stockTransferReversal.JobNo,
                    ProductCode = summary.ProductCode,
                    SupplierId = summary.SupplierId,
                    CustomerCode = summary.CustomerCode,
                    Ownership = Ownership.EHP,
                    JobDate = stockTransferReversal.CreatedDate,
                    Act = InventoryTransactionType.ReversalOfPurchaseOfAgeStock,
                    Qty = Math.Abs(summary.TotalQty),
                    BalanceQty = lastInvTransPurchase.BalanceQty - summary.TotalQty,
                };

                if(newInvTransPurchase.BalanceQty < 0)
                    throw new ApplicationError($"Reversal fail as Balance Qty of {summary.ProductCode} in Inventory Warehouse Transaction by supplier< 0");

                repository.InventoryTransactions.AddTransaction(newInvTransPurchase);

                // step 4.6 : Insert to billing log
                // Insert to billing log if VMI supplier
                var paradigm = repository.StockTransferReversals.GetSupplierParadigm(summary.SupplierId, summary.CustomerCode);
                if(paradigm.Substring(1, 1).ToUpper() == "V")
                {
                    await repository.BillingLogs.AddNewBillingLog(
                        stockTransferReversal.JobNo,
                        summary.CustomerCode,
                        summary.SupplierId,
                        summary.ProductCode,
                        stockTransferReversal.JobNo,
                        summary.TotalQty * -1
                    );
                }

                await repository.SaveChangesAsync(cancellationToken);
            }

            // Step 5 : Update Stock Transfer Reversal Master
            stockTransferReversal.Complete(request.UserCode, dateTimeService.Now);
            await repository.StockTransferReversals.Update(stockTransferReversal);

            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();
        }
        catch(Exception)
        {
            repository.RollbackTransaction();
            throw;
        }

        var updatedReversal = await repository.StockTransferReversals.GetStockTransferReversal(request.JobNo)
            ?? throw new ApplicationError($"Unknown StockTransfer reversal JobNo {request.JobNo}.");
        return updatedReversal;
    }
}
