using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.UseCases.StockTransferReversals;
using Application.UseCases.StockTransferReversals.Queries.GetStockTransferReversalDetails;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.StockTransferReversalItems.Commands.DeleteStockTransferReversalItemCommand;

public class DeleteStockTransferReversalItemCommand : IRequest<StockTransferReversalDetailsDto>
{
    public string JobNo { get; set; } = null!;
    public string PID { get; set; } = null!;
}

public class DeleteStockTransferReversalItemCommandHandler : IRequestHandler<DeleteStockTransferReversalItemCommand, StockTransferReversalDetailsDto>
{
    private readonly IRepository repository;
    private readonly IMediator mediator;

    public DeleteStockTransferReversalItemCommandHandler(IRepository repository, IMediator mediator)
    {
        this.repository = repository;
        this.mediator = mediator;
    }

    public async Task<StockTransferReversalDetailsDto> Handle(DeleteStockTransferReversalItemCommand request, CancellationToken cancellationToken)
    {
        repository.BeginTransaction();
        try
        {
            var stockTransferReversal = await repository.StockTransferReversals.GetStockTransferReversal(request.JobNo)
                ?? throw new ApplicationError($"StockTransfer reversal {request.JobNo} does not exist.");

            if(stockTransferReversal.Status == StockTransferReversalStatus.Completed
                || stockTransferReversal.Status == StockTransferReversalStatus.Cancelled)
                throw new ApplicationError($"Cannot modify completed or cancelled stock transfer reversal.");

            await repository.StockTransferReversals.DeleteDetail(request.JobNo, request.PID);
            await repository.SaveChangesAsync();

            var itemsCount = repository.StockTransferReversals.GetDetailsCount(request.JobNo);
            if(itemsCount == 0)
                stockTransferReversal.Status = StockTransferReversalStatus.New;
            else
                stockTransferReversal.Status = StockTransferReversalStatus.Processing;

            await repository.StockTransferReversals.Update(stockTransferReversal);
            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();

        }
        catch(Exception)
        {
            repository.RollbackTransaction();
            throw;
        }

        return await mediator.Send(new GetStockTransferReversalDetailsQuery
        {
            JobNo = request.JobNo
        });
    }
}
