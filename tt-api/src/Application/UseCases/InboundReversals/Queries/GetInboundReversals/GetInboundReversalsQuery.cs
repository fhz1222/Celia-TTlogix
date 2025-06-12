using Application.Common.Models;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.InboundReversals.Queries.GetInboundReversals;

public class GetInboundReversalsQuery : IRequest<PaginatedList<InboundReversalDto>>
{
    public PaginationQuery Pagination { get; set; } = null!;
    public GetInboundReversalsDtoFilter Filter { get; set; } = null!;
    public string? OrderByExpression { get; set; }
    public bool OrderByDescending { get; set; }
}

public class GetInboundReversalsQueryHandler : IRequestHandler<GetInboundReversalsQuery, PaginatedList<InboundReversalDto>>
{
    private readonly IInboundReversalRepository inboundReversalRepository;

    public GetInboundReversalsQueryHandler(IInboundReversalRepository inboundReversalsRepository)
    {
        inboundReversalRepository = inboundReversalsRepository;
    }

    public async Task<PaginatedList<InboundReversalDto>> Handle(GetInboundReversalsQuery request, CancellationToken cancellationToken)
    {
        var result = inboundReversalRepository.GetInboundReversals(request.Pagination, request.Filter, request.OrderByExpression, request.OrderByDescending);
        return await Task.FromResult(result);
    }
}
