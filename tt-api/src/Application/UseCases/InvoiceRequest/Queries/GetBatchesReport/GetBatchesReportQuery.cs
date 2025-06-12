using Application.Common.Models;
using Application.Interfaces.Gateways;
using Application.Interfaces.Repositories;
using Application.UseCases.InvoiceRequest.Queries.GetBatches;
using MediatR;

namespace Application.UseCases.InvoiceRequest.Queries.GetBatchesReport;

public class GetBatchesReportQuery : IRequest<Stream>
{
    public GetBatchesQueryFilter Filter { get; set; } = null!;
    public PaginationQuery Pagination { get; set; } = default!;
}

public class GetBatchesReportQueryHandler : IRequestHandler<GetBatchesReportQuery, Stream>
{
    private readonly IInvoiceRequestRepository repository;
    private readonly IExcelWriter excelWriter;

    public GetBatchesReportQueryHandler(IInvoiceRequestRepository repository, IExcelWriter excelWriter)
    {
        this.repository = repository;
        this.excelWriter = excelWriter;
    }

    public async Task<Stream> Handle(GetBatchesReportQuery request, CancellationToken cancellationToken)
    {
        var data = await repository.GetBatches(request.Filter, request.Pagination);
        var excel = excelWriter.GetBatchesExcel(data.Items);
        return excel;
    }
}
