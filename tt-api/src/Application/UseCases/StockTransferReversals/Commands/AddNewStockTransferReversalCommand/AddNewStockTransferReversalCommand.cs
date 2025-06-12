using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.StockTransferReversals.Commands.AddNewStockTransferReversalCommand;

public class AddNewStockTransferReversalCommand : IRequest<StockTransferReversal>
{
    public string StfJobNo { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class AddNewStockTransferReversalCommandHandler : IRequestHandler<AddNewStockTransferReversalCommand, StockTransferReversal>
{
    private readonly IJobNumberGenerator jobNumberGenerator;
    private readonly IRepository repository;

    public AddNewStockTransferReversalCommandHandler(IJobNumberGenerator jobNumberGenerator, IRepository repository)
    {
        this.jobNumberGenerator = jobNumberGenerator;
        this.repository = repository;
    }

    public async Task<StockTransferReversal> Handle(AddNewStockTransferReversalCommand request, CancellationToken cancellationToken)
    {
        repository.BeginTransaction();
        try
        {
            var stockTransfer = await repository.StockTransferReversals.GetStockTransferInfo(request.StfJobNo)
                ?? throw new ApplicationError($"Unknown stock transfer JobNo {request.StfJobNo}.");

            if(stockTransfer.Type != StockTransferType.Over90Days
                && stockTransfer.Type != StockTransferType.EStockTransfer)
                throw new ApplicationError($"Failed to reverse! Only stock over 90 days or EStock can be reversed.");

            if(stockTransfer.Status != StockTransferStatus.Completed)
                throw new ApplicationError($"Stock transfer must be completed.");

            var jobNumber = jobNumberGenerator.GetJobNumber(repository.StockTransferReversals);
            var newStockTransferReversal = new StockTransferReversal
            {
                JobNo = jobNumber,
                StfJobNo = request.StfJobNo,
                CreatedBy = request.UserCode,
                RefNo = stockTransfer.RefNo ?? "",
                CustomerCode = stockTransfer.CustomerCode,
                WhsCode = stockTransfer.WhsCode,
                CreatedDate = DateTime.Now,
                Status = StockTransferReversalStatus.New,
                Reason = "",
                ConfirmedBy = "",
                ConfirmedDate = null,
                CancelledBy = "",
                CancelledDate = null,
            };

            await repository.StockTransferReversals.AddNew(newStockTransferReversal);
            repository.CommitTransaction();

            return newStockTransferReversal;
        }
        catch(Exception)
        {
            repository.RollbackTransaction();
            throw;
        }
    }
}
