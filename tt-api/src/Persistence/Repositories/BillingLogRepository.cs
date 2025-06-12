using Application.Exceptions;
using Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.Repositories;

public class BillingLogRepository : IBillingLogRepository
{
    private readonly AppDbContext context;
    private readonly AutoMapper.IMapper mapper;

    public BillingLogRepository(AppDbContext context, AutoMapper.IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public async Task AddNewBillingLog(
        string jobNo, string factoryID, string supplierID,
        string productCode, string strRefNo, decimal quantity)
    {
        var (costCurrency, costPrice) = RetrievePrice(factoryID, supplierID, productCode);
        var log = new BillingLog
        {
            JobNo = jobNo,
            FactoryId = factoryID,
            SupplierId = supplierID,
            ProductCode = productCode,
            RefNo = strRefNo,
            CostCurrency = costCurrency,
            CostPrice = costPrice,
            Quantity = quantity,
            BillingNo = ""
        };

        await context.BillingLogs.AddAsync(log);
    }

    private (string costCurrency, decimal costPrice) RetrievePrice(string factoryID, string supplierID, string productCode)
    {
        // If records are found but there exists more than 1 record, it means there is something wrong with the data
        var priceInfo = context.SupplierItems
            .AsNoTracking()
            .Where(x => x.FactoryId == factoryID)
            .Where(x => x.SupplierId == supplierID)
            .Where(x => x.ProductCode == productCode)
            .FirstOrDefault();

        if(priceInfo is null)
            throw new ApplicationError($"No price records found for {factoryID} {supplierID} {productCode}.");

        string costCurrency = string.Empty;
        decimal costPrice = 0;
        var today = DateTime.Today;

        var boolPriceSet = false;
        // Future
        if(priceInfo.FutureCostEffectiveDate.HasValue
            && today >= priceInfo.FutureCostEffectiveDate)
        {
            costCurrency = priceInfo.FutureCostCurrency ?? "";
            costPrice = priceInfo.FutureCost;
            boolPriceSet = true;
        }
        // Current
        if(priceInfo.CurrentCostEffectiveDate.HasValue
            && today >= priceInfo.CurrentCostEffectiveDate)
        {
            costCurrency = priceInfo.CurrentCostCurrency ?? "";
            costPrice = priceInfo.CurrentCost;
            boolPriceSet = true;
        }
        // Past
        if(!boolPriceSet
            && priceInfo.PastCostEffectiveDate.HasValue
            && today >= priceInfo.PastCostEffectiveDate)
        {
            costCurrency = priceInfo.PastCostCurrency ?? "";
            costPrice = priceInfo.PastCost;
            boolPriceSet = true;
        }

        if(!boolPriceSet)
        {
            costCurrency = priceInfo.CurrentCostCurrency ?? "";
            costPrice = priceInfo.CurrentCost;
        }

        return (costCurrency, costPrice);
    }
}
