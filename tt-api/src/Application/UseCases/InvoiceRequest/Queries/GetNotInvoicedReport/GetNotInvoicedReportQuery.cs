using Application.Interfaces.Gateways;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.InvoiceRequest.Queries.GetNotInvoicedReport;

public class GetNotInvoicedReportQuery : IRequest<Stream>
{
    public string FactoryId { get; set; } = default!;
    public string SupplierId { get; set; } = default!;
}

public class GetNotInvoicedReportQueryHandler : IRequestHandler<GetNotInvoicedReportQuery, Stream>
{
    private readonly IInvoiceRequestRepository repository;
    private readonly IExcelWriter excelWriter;

    public GetNotInvoicedReportQueryHandler(IInvoiceRequestRepository repository, IExcelWriter excelWriter)
    {
        this.repository = repository;
        this.excelWriter = excelWriter;
    }

    public async Task<Stream> Handle(GetNotInvoicedReportQuery r, CancellationToken cancellationToken)
    {
        var openRequests = await repository.GetOpenRequests(r.FactoryId, r.SupplierId);
        var excel = excelWriter.GetNotInvoicedExcel(openRequests);
        return excel;
    }
}
