using Application.Exceptions;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.StockTransferReversals.Commands.UpdateStockTransferReversalCommand;

public class UpdateStockTransferReversalCommand : IRequest<StockTransferReversal>
{
    public string JobNo { get; set; } = null!;
    public string RefNo { get; set; } = null!;
    public string? Reason { get; set; } = null!;
}

public class UpdateStockTransferReversalCommandHandler : IRequestHandler<UpdateStockTransferReversalCommand, StockTransferReversal>
{
    private readonly IRepository repository;

    public UpdateStockTransferReversalCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<StockTransferReversal> Handle(UpdateStockTransferReversalCommand request, CancellationToken cancellationToken)
    {
        repository.BeginTransaction();
        try
        {
            var stockTransferReversal = await repository.StockTransferReversals.GetStockTransferReversal(request.JobNo)
                ?? throw new ApplicationError($"Unknown stock transfer reversal JobNo {request.JobNo}.");

            if(stockTransferReversal.Status == StockTransferReversalStatus.Completed
                || stockTransferReversal.Status == StockTransferReversalStatus.Cancelled)
                throw new ApplicationError($"Cannot modify completed or cancelled stock transfer reversal.");

            stockTransferReversal.RefNo = request.RefNo;
            stockTransferReversal.Reason = request.Reason ?? "";

            await repository.StockTransferReversals.Update(stockTransferReversal);
            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();
            return stockTransferReversal;
        }
        catch(Exception)
        {
            repository.RollbackTransaction();
            throw;
        }
    }
}
