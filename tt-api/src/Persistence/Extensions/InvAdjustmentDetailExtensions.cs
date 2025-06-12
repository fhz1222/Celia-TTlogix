using Domain.Entities;
using Persistence.Entities;

namespace Persistence.Extensions
{
    internal static class InvAdjustmentDetailExtensions
    {
        internal static int GetDiffPkg(this TtInvAdjustmentDetail adjustmentItem)
        {
            if (adjustmentItem.NewQty == 0 && adjustmentItem.OldQty != 0) 
                return -1;

            if (adjustmentItem.NewQty != 0 && adjustmentItem.OldQty == 0)
                return 1;

            return 0;
        }
    }
}
