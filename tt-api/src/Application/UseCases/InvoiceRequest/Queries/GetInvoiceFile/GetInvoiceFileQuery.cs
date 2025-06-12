using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.InvoiceRequest.Queries.GetInvoiceFile;

public class GetInvoiceFileQuery : IRequest<(Stream file, string fileName)>
{
    public int InvoiceFileId { get; set; }
}

public class GetInvoiceFileQueryHandler : IRequestHandler<GetInvoiceFileQuery, (Stream file, string fileName)>
{
    private readonly IInvoiceRequestRepository repository;

    public GetInvoiceFileQueryHandler(IInvoiceRequestRepository repository) => this.repository = repository;

    public async Task<(Stream file, string fileName)> Handle(GetInvoiceFileQuery request, CancellationToken cancellationToken)
    {
        var (file, fileName) = await repository.GetInvoiceFile(request.InvoiceFileId);
        var stream = new MemoryStream(file);
        return (stream, fileName);
    }
}
