using Application.Exceptions;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.StockTransferReversals.Queries.GetStockTransferReversalDetails;

public class GetStockTransferReversalDetailsQuery : IRequest<StockTransferReversalDetailsDto>
{
    public string JobNo { get; set; } = null!;
}

public class GetStockTransferReversalDetailsHandler : IRequestHandler<GetStockTransferReversalDetailsQuery, StockTransferReversalDetailsDto>
{
    private readonly IStockTransferReversalRepository repository;

    public GetStockTransferReversalDetailsHandler(IStockTransferReversalRepository repository)
    {
        this.repository = repository;
    }

    public async Task<StockTransferReversalDetailsDto> Handle(GetStockTransferReversalDetailsQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetStockTransferReversal(request.JobNo)
            ?? throw new ApplicationError($"Unknown StockTransfer reversal JobNo {request.JobNo}.");

        var customerName = await repository.GetCustomerName(result.CustomerCode, result.WhsCode)
            ?? throw new ApplicationError($"Unknown customer code {result.CustomerCode} WHS {result.WhsCode}.");

        var stockTransfer = await repository.GetStockTransferInfo(result.StfJobNo)
            ?? throw new ApplicationError($"Unknown StockTransfer JobNo {result.StfJobNo}.");

        return new StockTransferReversalDetailsDto
        {
            Type = stockTransfer.Type,
            StfJobNo = result.StfJobNo,
            CreatedBy = result.CreatedBy,
            CreatedDate = result.CreatedDate,
            ConfirmedBy = result.ConfirmedBy,
            ConfirmedDate = result.ConfirmedDate,
            CustomerCode = result.CustomerCode,
            CustomerName = customerName,
            JobNo = result.JobNo,
            Reason = result.Reason,
            RefNo = result.RefNo,
            Status = result.Status,
            WhsCode = result.WhsCode,
        };
    }
}

