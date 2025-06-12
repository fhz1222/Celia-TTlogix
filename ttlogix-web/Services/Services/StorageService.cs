using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TT.Common;
using TT.Core.Entities;
using TT.Core.Enums;
using TT.Core.Interfaces;
using TT.Core.QueryFilters;
using TT.Core.QueryResults;
using TT.Services.Interfaces;
using TT.Services.Label;
using TT.Services.Models;
using TT.Services.Services.Utilities;

namespace TT.Services.Services
{
    public class StorageService : ServiceBase<StorageService>, IStorageService
    {
        public StorageService(ITTLogixRepository repository,
            ILabelProvider labelProvider,
            IOptions<AppSettings> appSettings,
            ILocker locker,
            IMapper mapper,
            ILogger<StorageService> logger) : base(locker, logger)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.labelProvider = labelProvider;
            this.appSettings = appSettings.Value;
        }

        public async Task<IEnumerable<StorageDetailDto>> GetStoragePutawayList(string inJobNo, int? lineItem)
        {
            return await repository.GetStoragePutawayList<StorageDetailDto>(inJobNo, lineItem);
        }

        public async Task<IEnumerable<StorageDetailWithPartInfoDto>> GetStorageDetailWithPartsInfoList(string outJobNo, int lineItem, string whsCode)
        {
            var outbound = await repository.GetOutboundAsync(outJobNo);
            var outboundDetail = await repository.GetOutboundDetailAsync(outJobNo, lineItem);
            var filter = new StorageDetailQueryFilter()
            {
                WHSCode = whsCode,
                CustomerCode = outbound.CustomerCode,
                ProductCode = outboundDetail.ProductCode,
                SupplierId = outboundDetail.SupplierID,
                LocationType = LocationType.Normal,
                Statuses = new StorageStatus[] { StorageStatus.Putaway },
            };
            if (outbound.TransType == OutboundType.Return)
                filter.Ownership = Ownership.Supplier;

            var result = (await repository.GetStorageDetailWithPartInfo(filter)).Select(i => mapper.Map<StorageDetailWithPartInfoDto>(i));
            return result.OrderBy(r => r.InboundDate);
        }

        public async Task<IEnumerable<STFStorageDetailWithPartInfoDto>> GetSTFStorageDetailList(string stockTransferJobNo, string inJobNo, string supplierId, string whsCode)
        {
            var stockTransfer = await repository.GetStockTransferAsync(stockTransferJobNo);
            var filter = new SFTStorageDetailQueryFilter()
            {
                WHSCode = whsCode,
                CustomerCode = stockTransfer.CustomerCode,
                InJobNo = inJobNo,
            };

            if (supplierId != null)
            {
                filter.SupplierIds = new string[] { supplierId };
            }

            var result = (await repository.GetSFTStorageDetailWithPartInfo(filter)).Select(i => mapper.Map<STFStorageDetailWithPartInfoDto>(i));
            return result.OrderBy(r => r.InboundDate);
        }

        public async Task<IEnumerable<SupplierDto>> GetStorageSupplierList(string customerCode, string whsCode)
        {
            return await repository.GetDistinctStorageSupplierList<SupplierDto>(customerCode, whsCode);
        }
        public async Task<IEnumerable<InJobNoDto>> GetStorageInJobNosList(string customerCode, string supplierId, string whsCode)
        {
            return await repository.GetDistinctStorageInJobNoList<InJobNoDto>(customerCode, supplierId, whsCode);
        }

        public async Task<bool> HasBondedStock(string outJobNo)
        {
            return await repository.StorageDetails().Where(s => s.OutJobNo == outJobNo && s.BondedStatus == (int)BondedStatus.Bonded).AnyAsync();
        }

        public async Task<Result<bool>> UpdateSellingPrice(IEnumerable<UpdateSellingPriceItemDto> sellingPriceItems)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                foreach (var item in sellingPriceItems)
                {
                    if (String.IsNullOrEmpty(item.PID))
                        continue;

                    var price = Math.Round(item.Price, 6, MidpointRounding.ToZero);

                    var storage = await repository.GetStorageDetailAsync(item.PID);
                    if (storage.Ownership == (int)Ownership.Supplier)
                        storage.SellingPrice = price;
                }
                await repository.SaveChangesAsync();
                return new SuccessResult<bool>(true);
            });
        }

        public async Task<Result<bool>> UpdateBuyingPrice(UpdateBuyingPriceItemDto data)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                if (!UtilityService.CURRENCIES.Contains(data.Currency))
                {
                    return new InvalidResult<bool>(new JsonResultError("InvalidCurrency").ToJson());
                }

                foreach (var priceItem in data.Prices)
                {
                    var inboundDetail = await repository.GetInboundDetailAsync(data.InJobNo, priceItem.LineItem);
                    var qty = inboundDetail.Qty;
                    var price = Math.Round(priceItem.BuyingPrice, 6, MidpointRounding.ToZero);

                    var storageDetailAll = repository.StorageDetails().Where(s => s.InJobNo == data.InJobNo && s.LineItem == inboundDetail.LineItem).ToList();
                    var storageDetailForBuyingPriceUpdate = storageDetailAll.Where(s => (s.Qty == 0 && (!s.BuyingPrice.HasValue || s.BuyingPrice == 0)) || s.Qty > 0);
                    foreach (var sd in storageDetailForBuyingPriceUpdate)
                    {
                        sd.BuyingPrice = price;
                    }
                    var storageDetailForSellingPriceUpdate = storageDetailAll.Where(s => !s.SellingPrice.HasValue || s.SellingPrice == 0);
                    foreach (var sd in storageDetailForSellingPriceUpdate)
                    {
                        sd.SellingPrice = price;
                    }
                    await repository.SaveChangesAsync();
                    inboundDetail.BuyingPricePerLine = qty * price;
                }
                var inbound = await repository.GetInboundAsync(data.InJobNo);
                if (inbound == null)
                {
                    return new NotFoundResult<bool>(new JsonResultError("RecordNotFound").ToJson());
                }
                inbound.Currency = data.Currency;
                await repository.SaveChangesAsync();

                return new SuccessResult<bool>(true);
            });
        }

        public async Task<Result<bool>> PrintLabels(string[] PID, ILabelFactory.LabelType labelType, string IP, int copies)
        {
            var labelPrinter = await repository.LabelPrinters().Where(x => x.IP == IP).FirstAsync();

            var factory = labelProvider.CreateFactory(labelPrinter);
            var PIDs = await GetPIDs(PID);
            if (PIDs.Count == 0)
            {
                return new InvalidResult<bool>(new JsonResultError("ListIsEmpty").ToJson());
            }
            foreach (var p in PIDs)
            {
                await factory.AddLabel(p, labelType);
            }
            try
            {
                await factory.Print(copies);
            }
            catch (PrinterUnavailableException)
            {
                return new InvalidResult<bool>(new JsonResultError("PrinterIsUnavailable").ToJson());
            }
            return new SuccessResult<bool>(true);
        }

        public async Task<Result<IEnumerable<ExternalQRCodeForInboundDetailDto>>> GetExternalQRStorageLabelsForInbound(string inJobNo, string supplierCode, string factoryId)
        {
            var data = await GetStoragePutawayList(inJobNo, null);
            var result = new List<ExternalQRCodeForInboundDetailDto>();
            foreach (var s in data)
            {
                var detail = await repository.GetInboundDetailAsync(inJobNo, s.LineItem);
                if (s.ExternalSystem == null)
                {
                    return new InvalidResult<IEnumerable<ExternalQRCodeForInboundDetailDto>>(new JsonResultError("MissingExternalPID").ToJson());
                }
                var code = s.ExternalSystem switch
                {
                    6 => $"<SS>{s.ExternalPID}\u0005{s.ProductCode}\u0005\u0005{(int)s.Qty}\u0005{s.LineItem}\u0005{supplierCode}",
                    7 => $"<E2>{factoryId};{s.ProductCode};{(int)s.Qty};{supplierCode};;{detail.ControlCode1};{s.ExternalPID}",
                    8 => $"<S5>\u0005{s.ExternalPID}\u0005{s.ProductCode}\u0005\u0005{(int)s.Qty}\u0005{s.LineItem}\u0005{supplierCode}",
                    _ => null
                };
                if (code == null)
                {
                    return new InvalidResult<IEnumerable<ExternalQRCodeForInboundDetailDto>>(new JsonResultError("UnrecognizedExternalSystem__", "externalSystem", s.ExternalSystem.ToString()).ToJson());
                }
                result.Add(new()
                {
                    Code = code,
                    LineItem = s.LineItem,
                    Name = $"{inJobNo}:{s.LineItem}"
                });

            }
            return new SuccessResult<IEnumerable<ExternalQRCodeForInboundDetailDto>>(result);
        }

        public async Task<Result<IEnumerable<StorageLabelDto>>> GetStorageLabels(string[] PID)
        {
            var storageAndParts = (await (from sd in repository.StorageDetails()
                                          join sm in repository.SupplierMasters() on new { sd.SupplierID, sd.CustomerCode } equals new { sm.SupplierID, CustomerCode = sm.FactoryID }
                                          join pm in repository.PartMasters() on new { sd.SupplierID, sd.ProductCode, sd.CustomerCode } equals new { pm.SupplierID, ProductCode = pm.ProductCode1, pm.CustomerCode } 
                                          join asn_t in repository.ASNDetails() on new { sd.InJobNo, sd.ProductCode } equals new { asn_t.InJobNo, asn_t.ProductCode } into asnjoin
                                          from asn in asnjoin.DefaultIfEmpty()
                                          where PID.Contains(sd.PID)
                                          select new { StorageDetail = sd, CompanyName = sm.CompanyName, ASNNo = asn == null ? null : asn.ASNNo, Description = pm.Description }).Distinct().ToListAsync());

            List<StorageDetail> PIDs = storageAndParts.Select(s => s.StorageDetail).Distinct().ToList();
            List<string> data = new List<string>();

            var factory = new QRCodeLabelFactory(repository, (string[] res) => data = new List<string>(res));
            foreach (var p in PIDs)
            {
                await factory.AddLabel(p, ILabelFactory.LabelType.SMALL);
            }
            await factory.Print(1);

            IEnumerable<QRCodeDto> codes = data.Select((d, i) => new QRCodeDto { Code = d, Name = PID[i] });
            IEnumerable<StorageLabelDto> labels = Enumerable.Empty<StorageLabelDto>();

            foreach (var sp in storageAndParts.Distinct())
            {
                var inventoryCtrl = await repository.GetInventoryControlAsync(sp.StorageDetail.CustomerCode);
                var label = new StorageLabelDto()
                {
                    Code = codes.SingleOrDefault(c => c.Name == sp.StorageDetail.PID),
                    PartNo = sp.StorageDetail.ProductCode,
                    Qty = sp.StorageDetail.Qty,
                    SupplierID = sp.StorageDetail.SupplierID,
                    SupplierName = sp.CompanyName,
                    DateRecieved = sp.StorageDetail.InboundDate,
                    InboundJobNo = sp.StorageDetail.InJobNo,
                    Pid = sp.StorageDetail.PID,
                    ControlCode1Header = (await repository.GetControlCodeAsync(inventoryCtrl.CC1Type)).Name,
                    ControlCode1 = sp.StorageDetail.ControlCode1,
                    ControlCode2Header = (await repository.GetControlCodeAsync(inventoryCtrl.CC2Type)).Name,
                    ControlCode2 = sp.StorageDetail.ControlCode2,
                    ASNNo = sp.ASNNo,
                    CustomerID = sp.StorageDetail.CustomerCode,
                    Description = sp.Description
                };
                labels = labels.Append(label);
            }
            return new SuccessResult<IEnumerable<StorageLabelDto>>(labels);
        }

        private async Task<List<StorageDetail>> GetPIDs(string[] PID)
        {
            return await repository.StorageDetails().Where(x => PID.Contains(x.PID)).OrderBy(x => x.PID).ToListAsync();
        }

        private readonly ITTLogixRepository repository;
        private readonly IMapper mapper;
        private readonly ILabelProvider labelProvider;
        private readonly AppSettings appSettings;
    }
}
