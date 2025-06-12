namespace Application.Interfaces.Repositories;

public interface IBillingLogRepository
{
    Task AddNewBillingLog(string jobNo, string factoryID, string supplierID, string productCode, string strRefNo, decimal quantity);
}
