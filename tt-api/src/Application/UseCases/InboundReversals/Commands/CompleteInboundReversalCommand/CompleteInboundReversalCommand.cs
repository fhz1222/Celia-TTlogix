using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.InboundReversals.Commands.CompleteInboundReversalCommand;

public class CompleteInboundReversalCommand : IRequest<InboundReversal>
{
    public string JobNo { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class CompleteInboundReversalCommandHandler : IRequestHandler<CompleteInboundReversalCommand, InboundReversal>
{
    private readonly IRepository repository;
    private IDateTime dateTimeService;

    public CompleteInboundReversalCommandHandler(IRepository repository, IDateTime dateTimeService)
    {
        this.repository = repository;
        this.dateTimeService = dateTimeService;
    }

    public async Task<InboundReversal> Handle(CompleteInboundReversalCommand request, CancellationToken cancellationToken)
    {
        if(!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new ApplicationError($"User {request.UserCode} does not exist.");

        repository.BeginTransaction();
        try
        {
            // Step 1 : Get Inbound Reversal Master
            var inboundReversal = await repository.InboundReversals.GetInboundReversal(request.JobNo)
                ?? throw new ApplicationError($"Unknown inbound reversal JobNo {request.JobNo}.");

            if(inboundReversal.Status == InboundReversalStatus.New)
                throw new ApplicationError($"Cannot complete empty inbound reversal.");

            if(inboundReversal.Status == InboundReversalStatus.Completed)
                throw new ApplicationError($"Cannot modify completed inbound reversal.");

            if(inboundReversal.Status == InboundReversalStatus.Cancelled)
                throw new ApplicationError($"Cannot modify cancelled inbound reversal.");

            // Step 2 : Get Inbound Reversal Detail
            var details = repository.InboundReversals.GetInboundReversalDetails(inboundReversal.JobNo);
            
            // Step 3 : Perform Update StorageDetail
            foreach(var detail in details)
            {
                // Step 3.1 : Get Inbound Reversal Detail Instance
                // Step 3.2 : Get Storage Detail Instance
                var pallet = repository.StorageDetails.GetPalletDetail(detail.Pid)
                    ?? throw new ApplicationError($"Unknown PID {detail.Pid}.");

                // Step 3.4 : Verify Record
                if(pallet.WhsCode != inboundReversal.WhsCode)
                    throw new ApplicationError($"Invalid Warehouse Code ({pallet.WhsCode}), Unable to reversal {detail.JobNo}-{detail.ProductCode}-{detail.Pid}");

                if(pallet.Qty != detail.OriginalQty)
                    throw new ApplicationError($"Invalid Qty, Unable to reversal {detail.JobNo}({detail.Pid})-{detail.ProductCode}");

                if(pallet.Status != StorageStatus.Putaway)
                    throw new ApplicationError($"{pallet.Status} item, Unable to reversal {detail.JobNo}({detail.Pid})-{detail.ProductCode}");

                // Step 3.5 : Update Storage Detail
                pallet.Qty = 0;
                pallet.Status = StorageStatus.Reversal;

                await repository.StorageDetails.Update(pallet);
            }

            await repository.SaveChangesAsync(cancellationToken);

            // Step 4.1 : Get Inbound Reversal Detail
            var summaries = await repository.InboundReversals.GetInboundReversalSummary(inboundReversal.JobNo);

            foreach (var summary in summaries)
            {
                // Step 4.2 : Adjust Inventory Detail
                var invItem = repository.Inventory.GetInventoryItem(inboundReversal.WhsCode, summary.CustomerCode, summary.SupplierId, summary.ProductCode, summary.Ownership)
                    ?? throw new ApplicationError($"Fail to retrive Inventory Instance for {summary.JobNo}-{summary.ProductCode}");

                invItem.OnHandQty -= summary.TotalDifferent;
                invItem.OnHandPkg -= summary.TotalDiffPkg;

                await repository.Inventory.Update(invItem);

                // step 4.3 : Insert into InvTransactionPerSupplier
                var lastInvTrans = repository.InventoryTransactions.GetLatestTransactionPerSupplier(summary.CustomerCode, summary.ProductCode, summary.SupplierId, summary.Ownership)
                    ?? throw new ApplicationError($"No Last Inventory Transaction was found for {summary.JobNo}-{summary.ProductCode}");

                var newInvTrans = new InventoryTransactionPerSupplier
                {
                    JobNo = inboundReversal.JobNo,
                    ProductCode = summary.ProductCode,
                    SupplierId = summary.SupplierId,
                    CustomerCode = summary.CustomerCode,
                    Ownership = summary.Ownership,
                    JobDate = inboundReversal.CreatedDate,
                    Act = InventoryTransactionType.ReversalOfInbound,
                    Qty = Math.Abs(summary.TotalDifferent),
                    BalanceQty = lastInvTrans.BalanceQty - summary.TotalDifferent,
                };

                if(newInvTrans.BalanceQty < 0)
                    throw new ApplicationError($"Reversal fail as Balance Qty of {summary.ProductCode} in Inventory Warehouse Transaction by supplier< 0");

                repository.InventoryTransactions.AddTransaction(newInvTrans);

                // step 4.4 : Insert to billing log
                // Insert to billing log if VMI supplier & Inbound type = 2 (Return)
                var info = await repository.InboundReversals.GetInboundInfo(inboundReversal.InJobNo)
                    ?? throw new ApplicationError($"Failed to retrieve inbound {inboundReversal.InJobNo}.");

                if(info.Type == InboundType.Return
                    && summary.Ownership == Ownership.Supplier)
                {
                    var paradigm = repository.InboundReversals.GetSupplierParadigm(summary.SupplierId, summary.CustomerCode);
                    if(paradigm.Substring(1, 1).ToUpper() == "V")
                    {
                        await repository.BillingLogs.AddNewBillingLog(
                            inboundReversal.JobNo,
                            summary.CustomerCode,
                            summary.SupplierId,
                            summary.CustomerCode,
                            inboundReversal.InJobNo,
                            summary.TotalDifferent * -1 //Make it Positive as TotalDifferent always negative
                        );
                    }
                }

                await repository.SaveChangesAsync(cancellationToken);
            }

            // Step 5.1 : Get Inbound Reversal Detail
            var productSummaries = await repository.InboundReversals.GetInboundReversalSummaryByProduct(inboundReversal.JobNo)
                ?? throw new ApplicationError($"Failed to retrieve product summary list for inbound reversal {inboundReversal.JobNo}.");

            foreach(var summary in productSummaries)
            {
                // Step 5.2 : Insert into Inventory Transaction
                var lastInvTrans = repository.InventoryTransactions.GetLatestTransaction(summary.CustomerCode, summary.ProductCode); ;

                var balancePkg = 0;
                var balanceQty = 0;
                if(lastInvTrans is null)
                {
                    balancePkg = summary.TotalDiffPkg;
                    balanceQty = Math.Abs(summary.TotalDifferent);
                }
                else
                {
                    balancePkg = lastInvTrans.BalancePkg - summary.TotalDiffPkg;
                    balanceQty = lastInvTrans.BalanceQty - summary.TotalDifferent;

                    if(balanceQty < 0)
                        throw new ApplicationError($"Reversal fail as Balance Qty of {summary.ProductCode} in Inventory Transaction < 0");
                }

                var newInvTrans = new InventoryTransaction
                {
                    JobNo = inboundReversal.JobNo,
                    CustomerCode = inboundReversal.CustomerCode,
                    ProductCode = summary.ProductCode,
                    Qty = Math.Abs(summary.TotalDifferent),
                    Pkg = Math.Abs(summary.TotalDiffPkg),
                    JobDate = inboundReversal.CreatedDate,
                    Act = InventoryTransactionType.ReversalOfInbound,
                    BalancePkg = balancePkg,
                    BalanceQty = balanceQty
                };

                repository.InventoryTransactions.AddTransaction(newInvTrans);

                // Step 5.3 : Insert into Inventory Warehouse Transaction
                var lastWhsTrans = repository.InventoryTransactions.GetLatestTransactionPerWhsCode(summary.CustomerCode, summary.ProductCode, inboundReversal.WhsCode);

                var balancePkgWHS = 0;
                var balanceQtyWHS = 0;
                if(lastWhsTrans is null)
                {
                    balancePkgWHS = summary.TotalDiffPkg;
                    balanceQtyWHS = Math.Abs(summary.TotalDifferent);
                }
                else
                {
                    balancePkgWHS = lastWhsTrans.BalancePkg - summary.TotalDiffPkg;
                    balanceQtyWHS = lastWhsTrans.BalanceQty - summary.TotalDifferent;

                    if(balanceQtyWHS < 0)
                        throw new ApplicationError($"Reversal fail as Balance Qty of {summary.ProductCode} in Inventory Warehouse Transaction < 0");
                }

                var newWhsTrans = new InventoryTransactionPerWhsCode
                {
                    JobNo = inboundReversal.JobNo,
                    CustomerCode = inboundReversal.CustomerCode,
                    ProductCode = summary.ProductCode,
                    WhsCode = inboundReversal.WhsCode,
                    Qty = Math.Abs(summary.TotalDifferent),
                    Pkg = Math.Abs(summary.TotalDiffPkg),
                    JobDate = inboundReversal.CreatedDate,
                    Act = InventoryTransactionType.ReversalOfInbound,
                    BalancePkg = balancePkgWHS,
                    BalanceQty = balanceQtyWHS
                };

                repository.InventoryTransactions.AddTransaction(newWhsTrans);

                await repository.SaveChangesAsync(cancellationToken);
            }

            // Step 6 : Update Inbound Reversal Master
            inboundReversal.Complete(request.UserCode, dateTimeService.Now);
            await repository.InboundReversals.UpdateInboundReversal(inboundReversal);

            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();
        }
        catch(Exception)
        {
            repository.RollbackTransaction();
            throw;
        }

        var updatedReversal = await repository.InboundReversals.GetInboundReversal(request.JobNo)
            ?? throw new ApplicationError($"Unknown inbound reversal JobNo {request.JobNo}.");
        return updatedReversal;
    }
}
