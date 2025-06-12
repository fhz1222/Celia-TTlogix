using Application.UseCases.AdjustmentItems;
using Application.UseCases.Adjustments.Commands.CompleteAdjustmentCommand;
using Application.UseCases.Adjustments.Queries.GetAdjustmentPalletsInILog;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IAdjustmentItemRepository
{
    List<AdjustmentItem> GetAdjustmentItems(string jobNo, string? orderBy, bool orderByDescending);
    AdjustmentItemWithPalletDto? GetAdjustmentItemDetails(string jobNo, int lineItem);
    Task AddNew(AdjustmentItem newObject, string userCode, DateTime dateTime);
    Task Update(AdjustmentItem updatedObject, string userCode, DateTime dateTime);
    Task Delete(string jobNo, int lineItem);
    int GetLastLineItemNumber(string jobNo);
    bool PalletAppearsInAdjustment(string jobNo, string pallet);
    bool PalletAppearsInOutgoingAdjustment(string pallet);
    IEnumerable<AdjustmentItemSummaryDto> GetAdjustmentItemGroupedData(string jobNo);
    IEnumerable<AdjustmentItemSummaryByProductDto> GetAdjustmentItemByProductGroupedData(string jobNo);
    IEnumerable<AdjustmentPalletDto> GetAdjustmentPalletsOnLocationCategory(string jobNo, int locationCategoryId);
}
