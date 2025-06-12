using Application.Common.Models;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Inventory.Queries.GetInventoryItems;

public class GetInventoryItemsQuery : IRequest<PaginatedList<InventoryItemDto>>
{
    public PaginationQuery Pagination { get; set; } = null!;
    public InventoryItemDtoFilter Filter { get; set; } = null!;
    public string? OrderByExpression { get; set; }
    public bool OrderByDescending { get; set; }
}

public class GetInventoryItemsQueryHandler : IRequestHandler<GetInventoryItemsQuery, PaginatedList<InventoryItemDto>>
{
    private readonly IInventoryRepository inventoryRepository;

    public GetInventoryItemsQueryHandler(IInventoryRepository inventoryRepository)
    {
        this.inventoryRepository = inventoryRepository;
    }

    public async Task<PaginatedList<InventoryItemDto>> Handle(GetInventoryItemsQuery request, CancellationToken cancellationToken)
    {
        PaginatedList<InventoryItemDto> result = inventoryRepository.GetInventoryItems(request.Pagination,
            request.Filter, InventoryItemDto.GetOrderByFunction(request.OrderByExpression), request.OrderByDescending);
        return await Task.FromResult(result);
    }
}
