using Application.UseCases.RelocationLogs;
using Persistence.PetaPoco.Models;

namespace Persistence.Extensions
{
    internal static class RelocationLogExtensions
    {
        internal static RelocationLogDto SetPalletDetails(this RelocationLogDto relocationObj, string supplierId, string productCode, decimal qty)
        {
            relocationObj.SupplierId = supplierId;
            relocationObj.ProductCode = productCode;
            relocationObj.Qty = (int) qty;
            return relocationObj;
        }

        internal static RelocationLogDto SetCustomerCode(this RelocationLogDto relocationObj, TT_Customer customer)
        {
            relocationObj.CustomerCode = customer.Code;
            relocationObj.CustomerName = customer.Name;
            return relocationObj;
        }
    }
}
