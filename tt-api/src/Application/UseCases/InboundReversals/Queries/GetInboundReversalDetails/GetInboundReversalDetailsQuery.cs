using Application.Exceptions;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.InboundReversals.Queries.GetInboundReversalDetails;

public class GetInboundReversalDetailsQuery : IRequest<InboundReversalDetailsDto>
{
    public string JobNo { get; set; } = null!;
}

public class GetInboundReversalDetailsHandler : IRequestHandler<GetInboundReversalDetailsQuery, InboundReversalDetailsDto>
{
    private readonly IInboundReversalRepository repository;

    public GetInboundReversalDetailsHandler(IInboundReversalRepository repository)
    {
        this.repository = repository;
    }

    public async Task<InboundReversalDetailsDto> Handle(GetInboundReversalDetailsQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetInboundReversal(request.JobNo)
            ?? throw new ApplicationError($"Unknown inbound reversal JobNo {request.JobNo}.");

        var customerName = await repository.GetCustomerName(result.CustomerCode, result.WhsCode)
            ?? throw new ApplicationError($"Unknown customer code {result.CustomerCode} WHS {result.WhsCode}.");

        var inbound = await repository.GetInboundInfo(result.InJobNo)
            ?? throw new ApplicationError($"Unknown inbound JobNo {result.InJobNo}.");

        var supplierName = await repository.GetSupplierName(result.SupplierId, inbound.CustomerCode)
            ?? throw new ApplicationError($"Unknown supplier id {result.SupplierId} Customer {result.CustomerCode}.");

        return new InboundReversalDetailsDto
        {
            InJobNo = result.InJobNo,
            CreatedBy = result.CreatedBy,
            CreatedDate = result.CreatedDate,
            CustomerCode = result.CustomerCode,
            CustomerName = customerName,
            JobNo = result.JobNo,
            Reason = result.Reason,
            RefNo = result.RefNo,
            Status = result.Status,
            SupplierId = result.SupplierId,
            SupplierName = supplierName,
            WhsCode = result.WhsCode,
        };
    }
}

