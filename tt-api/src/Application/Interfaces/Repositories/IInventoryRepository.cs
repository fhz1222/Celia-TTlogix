using Application.Common.Models;
using Application.UseCases.Inventory;
using Application.UseCases.Inventory.Queries.GetInventoryItems;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Interfaces.Repositories;

public interface IInventoryRepository
{
    PaginatedList<InventoryItemDto> GetInventoryItems(PaginationQuery pagination, InventoryItemDtoFilter filter, Func<InventoryItemDto, object>? orderBySelector, bool orderByDescending);
    InventoryItem? GetInventoryItem(string whsCode, string customerCode, string supplierId, string productCode, Ownership ownership);
    Task Update(InventoryItem inventoryItem);
}