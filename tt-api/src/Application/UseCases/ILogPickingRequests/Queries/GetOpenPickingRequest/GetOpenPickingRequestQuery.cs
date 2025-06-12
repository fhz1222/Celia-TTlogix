using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.ILogPickingRequests.Queries.GetOpenPickingRequest;

public class GetOpenPickingRequestQuery : IRequest<string?>
{
    public string OutboundJobNo { get; set; }
}

public class GetOpenPickingRequestQueryHandler : IRequestHandler<GetOpenPickingRequestQuery, string?>
{
    private readonly IILogPickingRequestRepository repository;

    public GetOpenPickingRequestQueryHandler(IILogPickingRequestRepository repository)
    {
        this.repository = repository;
    }

    public async Task<string?> Handle(GetOpenPickingRequestQuery request, CancellationToken cancellationToken)
    {
        return repository.GetOpenRequest(request.OutboundJobNo)?.PickingRequestId;
    }
}
