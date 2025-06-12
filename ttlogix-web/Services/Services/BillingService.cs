using System;
using System.Threading.Tasks;
using TT.Core.Entities;
using TT.Core.Interfaces;
using TT.Services.Interfaces;

namespace TT.Services.Services
{
    public class BillingService : IBillingService
    {
        public BillingService(ITTLogixRepository repository)
        {
            this.repository = repository;
        }

        public async Task WriteToBillingLog(string jobNo, string factoryID, string supplierID, string productCode, string refNo, decimal qty, string billingNo = "")
        {
            // Retrieve the price for the productCode, by its effective date, factoryId and supplierID
            var priceData = await RetrievePrice(factoryID, supplierID, productCode);
            if (priceData != null)
            {
                var billingLog = new BillingLog()
                {
                    JobNo = jobNo,
                    FactoryID = factoryID,
                    SupplierID = supplierID,
                    ProductCode = productCode,
                    RefNo = refNo,
                    BillingNo = billingNo,
                    CostCurrency = priceData.CostCurrency,
                    CostPrice = priceData.CostPrice,
                    Quantity = qty
                };
                await repository.AddBillingLogAsync(billingLog);
            }
        }

        /// <summary>
        /// Gets the price and currency for a particular factory, supplier, part number
        /// Using today's date as the determinant to find out which cost (Past, Current, Future) to use
        /// </summary>
        /// <param name="factoryID">The factory of the product that we are trying to find a price for</param>
        /// <param name="supplierID">The supplier of the product that we are trying to find a price for</param>
        /// <param name="productCode">The product code of the product that we are trying to find a price for</param>
        /// <returns>True if a price can be determined and set</returns>
        private async Task<PriceDetails> RetrievePrice(string factoryID, string supplierID, string productCode)
        {
            var supplierItemMaster = await repository.GetSupplierItemMasterAsync(factoryID, supplierID, productCode);
            if (supplierItemMaster != null)
            {
                var today = DateTime.Now.Date;

                #region Future
                if (supplierItemMaster.FutureCostEffectiveDate.HasValue)
                {
                    if (supplierItemMaster.FutureCostEffectiveDate.Value <= today)
                    {
                        return new PriceDetails
                        {
                            CostPrice = supplierItemMaster.FutureCost,
                            CostCurrency = supplierItemMaster.FutureCostCurrency ?? string.Empty
                        };
                    }
                }
                #endregion

                #region Current
                if (supplierItemMaster.CurrentCostEffectiveDate.HasValue)
                {
                    if (supplierItemMaster.CurrentCostEffectiveDate.Value <= today)
                    {
                        return new PriceDetails
                        {
                            CostPrice = supplierItemMaster.CurrentCost,
                            CostCurrency = supplierItemMaster.CurrentCostCurrency ?? string.Empty
                        };
                    }
                }
                #endregion

                #region Past
                if (supplierItemMaster.PastCostEffectiveDate.HasValue)
                {
                    if (supplierItemMaster.PastCostEffectiveDate.Value <= today)
                    {
                        return new PriceDetails
                        {
                            CostPrice = supplierItemMaster.PastCost,
                            CostCurrency = supplierItemMaster.PastCostCurrency ?? string.Empty
                        };
                    }
                }
                #endregion

                return new PriceDetails
                {
                    CostPrice = supplierItemMaster.CurrentCost,
                    CostCurrency = supplierItemMaster.CurrentCostCurrency ?? string.Empty
                };
            }
            return null;
        }

        private class PriceDetails
        {
            public string CostCurrency { get; set; }
            public decimal? CostPrice { get; set; }
        }

        private readonly ITTLogixRepository repository;
    }
}
