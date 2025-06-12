using Application.UseCases.Inventory;

namespace Persistence.Extensions
{
    internal static class InventoryItemDtoExtensions
    {
        internal static InventoryItemDto SetBondedAndIncomingQty(this InventoryItemDto dto, int incoming, int bonded)
        {
            dto.BondedQty = bonded;
            dto.IncomingQty = incoming;
            return dto;
        }
    }
}
