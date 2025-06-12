using Domain.Entities;

namespace Persistence.Extensions
{
    internal static class AdjustmentExtensions
    {
        internal static Adjustment SetCustomerName(this Adjustment adjustmentObj, string customerName)
        {
            adjustmentObj.CustomerName = customerName;
            return adjustmentObj;
        }
        internal static AdjustmentItem SetSupplierId(this AdjustmentItem adjustmentItem, string supplierId)
        {
            adjustmentItem.SupplierId = supplierId;
            return adjustmentItem;
        }
    }
}
