using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.AdjustmentItems.Queries.GetAdjustmentItemsQuery;

public class GetAdjustmentItemsQuery : IRequest<List<AdjustmentItem>>
{
    public string JobNo { get; set; } = null!;
    public string? OrderByExpression { get; set; }
    public bool OrderByDescending { get; set; }
}

public class GetAdjustmentItemsQueryHandler : IRequestHandler<GetAdjustmentItemsQuery, List<AdjustmentItem>>
{
    private readonly IAdjustmentItemRepository repository;

    public GetAdjustmentItemsQueryHandler(IAdjustmentItemRepository repository)
    {
        this.repository = repository;
    }

    public async Task<List<AdjustmentItem>> Handle(GetAdjustmentItemsQuery request, CancellationToken cancellationToken)
    {
        List<AdjustmentItem> result = repository.GetAdjustmentItems(request.JobNo, request.OrderByExpression, request.OrderByDescending);

        return await Task.FromResult(result);
    }
}
