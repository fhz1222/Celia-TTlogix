using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServiceResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TT.Core.Entities;
using TT.Core.Enums;
using TT.Core.Interfaces;
using TT.Services.Interfaces;
using TT.Services.Models;
using TT.Services.Services.Utilities;

namespace TT.Services.Services
{
    public class PriceMasterService : ServiceBase<PriceMasterService>, IPriceMasterService
    {
        public PriceMasterService(ITTLogixRepository repository, ILocker locker, ILogger<PriceMasterService> logger) : base(locker, logger)
        {
            this.repository = repository;
        }

        public async Task<Result<bool>> UpdatePriceMasterOutbound(IEnumerable<UpdatePriceMasterPickingListDto> pickingListForPriceMasterDtos, string customerCode, string jobNo, string userCode)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                foreach (var item in pickingListForPriceMasterDtos)
                {
                    if (String.IsNullOrEmpty(item.PID))
                        continue;

                    if (item.Price == 0)
                        continue;

                    var batchUpdate = false;
                    var itemPrice = Math.Round(item.Price, 6, MidpointRounding.ToZero);
                    var priceMaster = await repository.GetPriceMasterAsync(customerCode, item.SupplierId, item.ProductCode);

                    if (priceMaster != null)
                    {
                        if (priceMaster.SellingPrice != itemPrice)
                            batchUpdate = true;

                        priceMaster.SellingPrice = itemPrice;
                        priceMaster.LastUpdatedOutbound = jobNo;
                        priceMaster.OutRevisedBy = userCode;
                        priceMaster.OutRevisedDate = DateTime.Now;
                        await repository.SaveChangesAsync();
                    }
                    else
                    {
                        priceMaster = new PriceMaster
                        {
                            CustomerCode = customerCode,
                            SupplierID = item.SupplierId,
                            ProductCode1 = item.ProductCode,
                            BuyingPrice = itemPrice,
                            SellingPrice = itemPrice,
                            CreatedBy = userCode,
                            OutRevisedBy = userCode,
                            OutRevisedDate = DateTime.Now,
                            LastUpdatedOutbound = jobNo,
                            LastUpdatedInbound = string.Empty,
                        };
                        await repository.AddPriceMasterAsync(priceMaster);
                        batchUpdate = true;
                    }

                    if (batchUpdate)
                    {
                        var storageStatuses = new StorageStatus[]
                        {
                         StorageStatus.Putaway, StorageStatus.Allocated, StorageStatus.Picked,  StorageStatus.Packed, StorageStatus.Quarantine
                        };
                        var storageDetails = await repository.StorageDetails().Where(sd => sd.Qty > 0
                                            && sd.Ownership == Ownership.Supplier
                                            && storageStatuses.Contains(sd.Status)
                                            && sd.ProductCode == item.ProductCode).ToListAsync();
                        foreach (var sd in storageDetails)
                        {
                            sd.SellingPrice = itemPrice;
                        }
                        await repository.SaveChangesAsync();
                    }
                }
                return new SuccessResult<bool>(true);
            });
        }

        public async Task<Result<bool>> UpdatePriceMasterInbound(IEnumerable<UpdatePriceMasterInboundDetailsDto> inboundDetailsForPriceMasterDtos, string customerCode, string supplierID, string jobNo, string userCode, string currency)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                foreach (var item in inboundDetailsForPriceMasterDtos)
                {
                    if (!UtilityService.CURRENCIES.Contains(currency))
                    {
                        return new InvalidResult<bool>(new JsonResultError("InvalidCurrency").ToJson());
                    }

                    if (item.Price == 0)
                        continue;

                    var itemPrice = Math.Round(item.Price, 6, MidpointRounding.ToZero);
                    var priceMaster = await repository.GetPriceMasterAsync(customerCode, supplierID, item.ProductCode);
                    if (priceMaster != null)
                    {
                        priceMaster.LastUpdatedInbound = jobNo;
                        priceMaster.Currency = currency;
                        priceMaster.BuyingPrice = itemPrice;
                        if (priceMaster.SellingPrice == 0)
                            priceMaster.SellingPrice = priceMaster.BuyingPrice;
                        priceMaster.RevisedBy = userCode;
                        priceMaster.RevisedDate = DateTime.Now;
                        await repository.SaveChangesAsync();
                    }
                    else
                    {
                        priceMaster = new PriceMaster
                        {
                            CustomerCode = customerCode,
                            SupplierID = supplierID,
                            ProductCode1 = item.ProductCode,
                            Currency = currency,
                            BuyingPrice = itemPrice,
                            SellingPrice = itemPrice,
                            CreatedBy = userCode,
                            LastUpdatedInbound = jobNo,
                            LastUpdatedOutbound = jobNo
                        };
                        await repository.AddPriceMasterAsync(priceMaster);
                    }
                }
                return new SuccessResult<bool>(true);
            });
        }

        private readonly ITTLogixRepository repository;
    }
}
