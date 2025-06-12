using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.AdjustmentItems.Queries.GetAdjustmentItemQuery;

public class GetAdjustmentItemQuery : IRequest<AdjustmentItemWithPalletDto>
{
    public string JobNo { get; set; } = null!;
    public int LineItem { get; set; }
}

public class GetAdjustmentItemQueryHandler : IRequestHandler<GetAdjustmentItemQuery, AdjustmentItemWithPalletDto>
{
    private readonly IAdjustmentItemRepository repository;

    public GetAdjustmentItemQueryHandler(IAdjustmentItemRepository repository)
    {
        this.repository = repository;
    }

    public async Task<AdjustmentItemWithPalletDto> Handle(GetAdjustmentItemQuery request, CancellationToken cancellationToken)
    {
        AdjustmentItemWithPalletDto result = repository.GetAdjustmentItemDetails(request.JobNo, request.LineItem);

        return await Task.FromResult(result);
    }
}
