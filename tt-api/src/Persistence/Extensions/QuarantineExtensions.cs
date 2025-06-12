using Application.UseCases.Quarantine;

namespace Persistence.Extensions
{
    internal static class QuarantineExtensions
    {
        internal static QuarantineItemDto SetAdditionalFields(this QuarantineItemDto quarantineItemObj, string customerName, string quarantineReason, int decimalNum)
        {
            quarantineItemObj.CustomerName = customerName;
            quarantineItemObj.Reason = quarantineReason;
            quarantineItemObj.DecimalNum = decimalNum;
            return quarantineItemObj;
        }
    }
}
