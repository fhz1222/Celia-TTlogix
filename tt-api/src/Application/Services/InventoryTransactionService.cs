using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Application.UseCases.Decants.Commands.CompleteDecantCommand;
using Domain.Entities;

namespace Application.Services;

internal class InventoryTransactionService : IInventoryTransactionService
{
    private readonly IRepository repository;

    public InventoryTransactionService(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task GenerateInventoryTransactionsOnAdjustmentComplete(string jobNo)
    {
        var adjustment = repository.Adjustments.GetAdjustmentDetails(jobNo);

        if (adjustment == null)
        {
            throw new UnknownJobNoException();
        }

        // Get inventory adjustment grouped data, then per each line get inventory and update
        var itemSummaries = repository.AdjustmentItems.GetAdjustmentItemGroupedData(adjustment.JobNo);
        foreach (var itemSummary in itemSummaries)
        {
            // add inventory transaction per supplier
            var latestTransaction = repository.InventoryTransactions.GetLatestTransactionPerSupplier(itemSummary.CustomerCode, itemSummary.ProductCode, itemSummary.SupplierId, itemSummary.Ownership);
            if (latestTransaction == null)
            {
                throw new TransactionNotFoundException("Fail to retrieve Last Inventory Warehouse Transaction by supplier (TT_InvTransactionPerSupplier)");
            }

            var newTransaction = itemSummary.CreateNewInventoryTransactionPerSupplier(adjustment, latestTransaction);

            if (newTransaction.BalanceQty < 0)
            {
                throw new AdjustmentFailException($"Adjustment fail as Balance Qty of {newTransaction.ProductCode} in Inventory Warehouse Transaction by supplier < 0");
            }

            repository.InventoryTransactions.AddTransaction(newTransaction);

        }
        // Get summary of inventory adjustment per product code and log into inventory transaction
        var itemPerProductSummaries = repository.AdjustmentItems.GetAdjustmentItemByProductGroupedData(adjustment.JobNo);
        foreach (var itemSummary in itemPerProductSummaries)
        {
            // add inventory transaction
            var latestTransaction = repository.InventoryTransactions.GetLatestTransaction(itemSummary.CustomerCode, itemSummary.ProductCode);
            if (latestTransaction == null && !itemSummary.PositiveAdjustment)
            {
                throw new TransactionNotFoundException("No last inventory transaction was found");
            }

            var newTransaction = itemSummary.CreateNewInventoryTransaction(adjustment, latestTransaction);

            if (newTransaction.BalanceQty < 0)
            {
                throw new AdjustmentFailException($"Adjustment fail as Balance Qty of {newTransaction.ProductCode} in Inventory Transaction < 0");
            }
            repository.InventoryTransactions.AddTransaction(newTransaction);

            // add inventory transaction per whscode
            var latestWhsTransaction = repository.InventoryTransactions.GetLatestTransactionPerWhsCode(itemSummary.CustomerCode, itemSummary.ProductCode, adjustment.WhsCode);
            if (latestWhsTransaction == null && !itemSummary.PositiveAdjustment)
            {
                throw new TransactionNotFoundException("No last inventory warehouse transaction was found");
            }

            var newWhsTransaction = itemSummary.CreateNewInventoryTransactionPerWhsCode(adjustment, latestWhsTransaction);

            if (newWhsTransaction.BalanceQty < 0)
            {
                throw new AdjustmentFailException($"Adjustment fail as Balance Qty of {newTransaction.ProductCode} in Inventory Warehouse Transaction < 0");
            }
            repository.InventoryTransactions.AddTransaction(newWhsTransaction);
        }
    }

    public async Task GenerateInventoryTransactionsOnDecantComplete(Decant decant)
    {
        var dataByProductCode = decant.Items.SelectMany(item => item.NewPallets)
            .GroupBy(p => p.ProductCode)
            .Select(p => new DecantItemSummaryByProductDto
            {
                JobNo = decant.JobNo,
                WhsCode = decant.WhsCode,
                CustomerCode = decant.CustomerCode,
                ProductCode = p.Key,
                JobDate = decant.CreatedDate,
                TotalPkg = p.Count()
            });

        foreach (var itemSummary in dataByProductCode)
        {
            // add inventory transaction
            var latestTransaction = repository.InventoryTransactions.GetLatestTransaction(decant.CustomerCode, itemSummary.ProductCode);
            if (latestTransaction == null)
            {
                throw new TransactionNotFoundException("No last inventory transaction was found");
            }

            var newTransaction = itemSummary.CreateNewInventoryTransaction(latestTransaction);
            repository.InventoryTransactions.AddTransaction(newTransaction);

            // add inventory transaction per whscode
            var latestWhsTransaction = repository.InventoryTransactions.GetLatestTransactionPerWhsCode(itemSummary.CustomerCode, itemSummary.ProductCode, decant.WhsCode);
            if (latestWhsTransaction == null && !itemSummary.PositiveAdjustment)
            {
                throw new TransactionNotFoundException("No last inventory warehouse transaction was found");
            }

            var newWhsTransaction = itemSummary.CreateNewInventoryTransactionPerWhsCode(latestWhsTransaction);

            repository.InventoryTransactions.AddTransaction(newWhsTransaction);
        }
    }
}
