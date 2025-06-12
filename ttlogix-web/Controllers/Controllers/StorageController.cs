using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TT.Common;
using TT.Controllers.Authorization;
using TT.Controllers.Extensions;
using TT.Services.Interfaces;
using TT.Services.Models;
using TT.Services.Services.Utilities;

namespace TT.Controllers.Controllers
{
    [ModuleAccessAuthorize(
        SystemModuleNames.STOCKTRANSFER + "," +
        SystemModuleNames.OUTBOUND + "," +
        SystemModuleNames.INBOUND + "," +
        SystemModuleNames.INVENTORY + "," +
        SystemModuleNames.LOADING
        )]
    [Authorize]
    [Route("api/storage")]
    [ApiController]
    public class StorageController : TTLogixControllerBase
    {
        public StorageController(IStorageService storageService, IInboundService inboundService)
            => (this.storageService, this.inboundService) = (storageService, inboundService);

        /// <summary>
        /// Gets the storage list of putaway items
        /// displayed on the inbound detail screen 
        /// displayed on InboundRemovePID screen
        /// </summary>
        /// <param name="inJobNo">Inbound JobNo</param>
        /// <param name="lineItem"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getStoragePutawayList")]
        public async Task<ActionResult<IEnumerable<StorageDetailDto>>> GetStoragePutawayList([RequiredAsJsonError] string inJobNo, int? lineItem)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return Ok(await storageService.GetStoragePutawayList(inJobNo, lineItem));
        }

        /// <summary>
        /// Gets the storage detail list visible in OutboundPickingList upper part of the screen
        /// Shows the list of available items to be picked from storage, for the prededined filter
        /// </summary>
        /// <param name="jobNo"></param>
        /// <param name="lineItem"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getStorageDetailWithPartsInfoList")]
        public async Task<ActionResult<IEnumerable<StorageDetailWithPartInfoDto>>> GetStorageDetailWithPartsInfoList(string jobNo, int lineItem)
            => Ok(await storageService.GetStorageDetailWithPartsInfoList(jobNo, lineItem, User.GetWHSCode()));

        /// <summary>
        /// Get available storage for Stock Transfer
        /// displayed on Stock Transfer detail > Add PID > Available List 
        /// Original path: LoadAvailableTdbg > m_oStorageController.GetSTFStorageDetail 
        /// </summary>
        /// <param name="jobNo">Stock transfer job no - required</param>
        /// <param name="inJobNo">optional</param>
        /// <param name="supplierId">optional</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getSTFStorageDetailList")]
        public async Task<ActionResult<IEnumerable<STFStorageDetailWithPartInfoDto>>> GetSTFStorageDetailList(
            [RequiredAsJsonError] string jobNo, string inJobNo = null, string supplierId = null)
            => Ok(await storageService.GetSTFStorageDetailList(jobNo, inJobNo, supplierId, User.GetWHSCode()));

        /// <summary>
        /// Get the distinct list of suppliers for storage, for selected customer code
        /// used in StockTransferDetailEntryEU > LoadSupplierDropDown > GetDistinctStorageSupplier
        /// </summary>
        /// <param name="customerCode">required</param>
        /// <param name="stockTransferJobNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getStorageSupplierList")]
        public async Task<ActionResult<IEnumerable<SupplierDto>>> GetStorageSupplierList([RequiredAsJsonError] string customerCode)
            => Ok(await storageService.GetStorageSupplierList(customerCode, User.GetWHSCode()));

        /// <summary>
        /// Get the distinct list of InJobNos for storage, for selected customer code
        /// used in StockTransferDetailEntryEU > LoadInJobNoDropDown > GetDistinctStorageInJobNo
        /// </summary>
        /// <param name="customerCode">required</param>
        /// <param name="supplierId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getStorageInJobNosList")]
        public async Task<ActionResult<IEnumerable<InJobNoDto>>> GetStorageInJobNosList([RequiredAsJsonError] string customerCode, [RequiredAsJsonError] string supplierId)
            => Ok(await storageService.GetStorageInJobNosList(customerCode, supplierId, User.GetWHSCode()));

        /// <summary>
        /// Check if there are any stock on storage marked as bonded
        /// original endpoint: m_oWebServiceCtrl.CheckBondedStock
        /// </summary>
        /// <param name="outJobNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("hasBondedStock")]
        public async Task<ActionResult<bool>> HasBondedStock([RequiredAsJsonError] string outJobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return Ok(await storageService.HasBondedStock(outJobNo));
        }

        /// <summary>
        /// Update selling price of multiple items
        /// original endpoint: Outbound detail > UpdateSellingPrice (btnCalculateOutboundValue_Click, "Calculate Outbound"
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updateSellingPrice")]
        public async Task<ActionResult> UpdateSellingPrice(UpdateSellingPriceItemsDto data)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await storageService.UpdateSellingPrice(data.Data));
        }

        /// <summary>
        /// Update buying price of selected inbound lines
        /// original endpoint: InboundDetail.UpdateBuyingPrice (originally not from API)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updateBuyingPrice")]
        public async Task<ActionResult> UpdateBuyingPrice([RequiredAsJsonError] UpdateBuyingPriceItemDto data)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await storageService.UpdateBuyingPrice(data));
        }

        /// <summary>
        /// Print PIDs labels on selected printer
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("printStorageLabels")]
        public async Task<ActionResult> PrintStorageLabels([RequiredAsJsonError] PrintLabelsDto data)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await storageService.PrintLabels(data.PID, Enum.Parse<ILabelFactory.LabelType>(data.Type), data.Printer, data.Copies));
        }

        /// <summary>
        /// Get External PIDs labels
        /// </summary>
        /// <param name="inJobNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getExternalQRStorageLabelsForInbound")]
        public async Task<ActionResult<IEnumerable<QRCodeDto>>> GetExternalQRStorageLabelsForInbound([RequiredAsJsonError] string inJobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }

            var inbound = await inboundService.GetInbound(inJobNo);
            return FromResult(await storageService.GetExternalQRStorageLabelsForInbound(inJobNo, inbound.SupplierID, inbound.CustomerCode));
        }

        /// <summary>
        /// Get PIDs labels
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getStorageLabels")]
        public async Task<ActionResult<IEnumerable<StorageLabelDto>>> GetStorageLabels([FromBody] GetQRsDto data)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await storageService.GetStorageLabels(data.PID));
        }
        //GetStorageLabels(pid[]) <QRCodeDto + PartNo SupplierID Description DateRecieved InboundJobNo Pid CustomerID>=<StorageLabelDto>

        private readonly IStorageService storageService;
        private readonly IInboundService inboundService;
    }
}
