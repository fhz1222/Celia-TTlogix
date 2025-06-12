using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.InvoiceRequest.Queries.GetFlow;

public class GetFlowQuery : IRequest<string>
{

}

public class GetFlowQueryHandler : IRequestHandler<GetFlowQuery, string>
{
    private readonly IInvoiceRequestRepository repository;

    public GetFlowQueryHandler(IInvoiceRequestRepository repository) => this.repository = repository;

    public Task<string> Handle(GetFlowQuery request, CancellationToken cancellationToken)
    {
        var flow = repository.GetFlow();
        return Task.FromResult(flow.ToString());
    }
}
