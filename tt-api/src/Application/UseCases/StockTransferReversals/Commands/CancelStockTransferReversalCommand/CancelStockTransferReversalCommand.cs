using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.StockTransferReversals.Commands.CancelStockTransferReversalCommand;

public class CancelStockTransferReversalCommand : IRequest<StockTransferReversal>
{
    public string JobNo { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class AddNewStockTransferReversalCommandHandler : IRequestHandler<CancelStockTransferReversalCommand, StockTransferReversal>
{
    private readonly IRepository repository;
    private IDateTime dateTimeService;

    public AddNewStockTransferReversalCommandHandler(IRepository repository, IDateTime dateTimeService)
    {
        this.repository = repository;
        this.dateTimeService = dateTimeService;
    }

    public async Task<StockTransferReversal> Handle(CancelStockTransferReversalCommand request, CancellationToken cancellationToken)
    {
        if(!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new ApplicationError($"User {request.UserCode} does not exist.");

        repository.BeginTransaction();
        try
        {
            var stockTransferReversal = await repository.StockTransferReversals.GetStockTransferReversal(request.JobNo)
                ?? throw new ApplicationError($"Unknown stock transfer reversal JobNo {request.JobNo}.");

            if(stockTransferReversal.Status == StockTransferReversalStatus.Completed
                || stockTransferReversal.Status == StockTransferReversalStatus.Cancelled)
                throw new ApplicationError($"Cannot modify completed or cancelled stock transfer reversal.");

            var anyDetailsExists = await repository.StockTransferReversals
                .AnyStockTransferReversalDetailsExists(stockTransferReversal.JobNo);

            if(anyDetailsExists)
                throw new ApplicationError($"Cannot cancel stock transfer reversal with details. Delete details and try again.");

            stockTransferReversal.Cancel(request.UserCode, dateTimeService.Now);
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
            ?? throw new ApplicationError($"Unknown stock Transfer reversal JobNo {request.JobNo}.");
        return updatedReversal;
    }
}
