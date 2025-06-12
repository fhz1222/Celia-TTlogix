using System.Threading.Tasks;

namespace TT.Services.Interfaces
{
    public interface IBillingService
    {
        Task WriteToBillingLog(string jobNo, string factoryID, string supplierID, string productCode, string refNo,
            decimal qty, string billingNo = "");
    }
}
