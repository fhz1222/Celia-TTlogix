using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.StockTransferReversalItems.Commands.AddNewStockTransferReversalItemCommand;

public class AddNewStockTransferReversalItemCommand : IRequest<Unit>
{
    public string[] PIDs { get; set; } = null!;
    public string JobNo { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class AddNewStockTransferReversalItemCommandHandler : IRequestHandler<AddNewStockTransferReversalItemCommand, Unit>
{
    private readonly IRepository repository;
    private readonly IDateTime dateTimeService;

    public AddNewStockTransferReversalItemCommandHandler(IDateTime dateTimeService, IRepository repository)
    {
        this.dateTimeService = dateTimeService;
        this.repository = repository;
    }

    public async Task<Unit> Handle(AddNewStockTransferReversalItemCommand request, CancellationToken cancellationToken)
    {
        if(!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new ApplicationError($"User {request.UserCode} does not exist.");

        repository.BeginTransaction();
        try
        {
            var stockTransferReversal = await repository.StockTransferReversals.GetStockTransferReversal(request.JobNo)
                ?? throw new ApplicationError($"Stock transfer reversal {request.JobNo} does not exist.");

            if(stockTransferReversal.Status == StockTransferReversalStatus.Completed
                || stockTransferReversal.Status == StockTransferReversalStatus.Cancelled)
                throw new ApplicationError($"Cannot modify completed or cancelled stock transfer reversal.");

            foreach(var PID in request.PIDs)
            {
                if(repository.StockTransferReversals.DetailExists(request.JobNo, PID))
                    throw new ApplicationError($"PID {PID} already exists in reversal {request.JobNo}");

                if(repository.StockTransferReversals.OutstandingReversalExistsForPID(PID))
                    throw new ApplicationError($"PID {PID} is pending for completion in other reversal job.");

                var info = await repository.StockTransferReversals.GetStockTransferDetailInfo(stockTransferReversal.StfJobNo, PID)
                    ?? throw new ApplicationError($"Error retrieving data for {request.JobNo} {PID}.");

                var newStockTransferReversalDetail = new StockTransferReversalDetail
                {
                    JobNo = request.JobNo,
                    Pid = PID,
                    ProductCode = info.ProductCode,
                    OriginalSupplierId = info.OriginalSupplierID,
                    OriginalLocationCode = info.LocationCode,
                    LocationCode = info.OriginalLocationCode,
                    OriginalWhscode = info.WHSCode,
                    Whscode = info.OriginalWHSCode,
                    TransferredBy = request.UserCode,
                    TransferredDate = DateTime.Now,
                };

                await repository.StockTransferReversals.AddNewDetail(newStockTransferReversalDetail);

                stockTransferReversal.Status = StockTransferReversalStatus.Processing;

                await repository.StockTransferReversals.Update(stockTransferReversal);
            }
            await repository.SaveChangesAsync(cancellationToken);

            repository.CommitTransaction();

            return Unit.Value;
        }
        catch(Exception)
        {
            repository.RollbackTransaction();
            throw;
        }
    }
}
