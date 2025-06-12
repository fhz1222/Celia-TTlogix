using Application.Common.Models;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Quarantine.Queries.GetQuarantineItems;

public class GetQuarantineItemsQuery : IRequest<PaginatedList<QuarantineItemDto>>
{
    public PaginationQuery Pagination { get; set; } = null!;
    public QuarantineItemDtoFilter Filter { get; set; }
    public string? OrderByExpression { get; set; }
    public bool OrderByDescending { get; set; }
}

public class GetQuarantineItemsQueryHandler : IRequestHandler<GetQuarantineItemsQuery, PaginatedList<QuarantineItemDto>>
{
    private readonly IQuarantineRepository repository;

    public GetQuarantineItemsQueryHandler(IQuarantineRepository repository)
    {
        this.repository = repository;
    }

    public async Task<PaginatedList<QuarantineItemDto>> Handle(GetQuarantineItemsQuery request, CancellationToken cancellationToken)
    {
        PaginatedList<QuarantineItemDto> result = repository.GetQuarantineItems(request.Pagination,
                    request.Filter, request.OrderByExpression, request.OrderByDescending);
        return await Task.FromResult(result);
    }
}
