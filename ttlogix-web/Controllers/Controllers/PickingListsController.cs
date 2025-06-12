using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TT.Common;
using TT.Controllers.Authorization;
using TT.Controllers.Extensions;
using TT.Core.QueryFilters;
using TT.Services.Interfaces;
using TT.Services.Models;
using TT.Services.Services.Utilities;

namespace TT.Controllers.Controllers
{
    [ModuleAccessAuthorize(
        SystemModuleNames.PICKINGLIST + "," +
        SystemModuleNames.PARTMASTER + "," +
        SystemModuleNames.STOCKTRANSFER + "," +
        SystemModuleNames.OUTBOUND + "," +
        SystemModuleNames.INBOUND + "," +
        SystemModuleNames.INVENTORY + "," +
        SystemModuleNames.LOADING
        )]
    [Route("api/pickingLists")]
    [ApiController]
    public class PickingListsController : TTLogixControllerBase
    {
        public PickingListsController(IPickingListService pickingListService)
            => this.pickingListService = pickingListService;

        /// <summary>
        /// Get picking list records for jobNo
        /// original endpoint: GetPickingListWithUOM(DataTable p_dtFilter, ref DataSet p_DataSet, DataTable p_dtOrderBy)
        /// </summary>
        /// <param name="jobNo"></param>
        /// <param name="lineItem">optional</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getPickingListWithUOM")]
        public async Task<ActionResult<IEnumerable<PickingListSimpleDto>>> GetPickingListWithUOM(string jobNo, int? lineItem)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return Ok(await pickingListService.GetPickingListWithUOM(jobNo, lineItem));
        }

        /// <summary>
        /// Check if there are any picking lists for the job/line item
        /// original endpoint: OutboundDetail btnModify_Click l_oPickingListCtrl.GetPickingList
        /// </summary>
        /// <param name="jobNo"></param>
        /// <param name="lineItem"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("hasPickingLists")]
        public async Task<ActionResult<bool>> HasPickingLists([RequiredAsJsonError] string jobNo, [RequiredAsJsonError] int lineItem)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return Ok(await pickingListService.HasPickingLists(jobNo, lineItem));
        }

        /// <summary>
        /// get text file with formatted picking data
        /// no endpoint, data received from DataDownloadController.GetPickingData(m_strJobNo, m_stProductionLine, m_iPickingItemType, out m_arySendData))
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getPickingDataToDownload")]
        public async Task<IActionResult> GetPickingDataToDownload([FromQuery] PickingListToDownloadQueryFilter queryFilter)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }

            var data = await pickingListService.GetPickingDataToDownload(queryFilter);
            return ToTextFileStreamResult(data, "PickingDataDownload.txt");
        }

        /// <summary>
        /// Auto Allocate operation for particular OutboundDetail JobNo/LineItem
        /// original endpoint: replaced 2 endpoints with one method
        /// AutoAllocatePickingListItem and AutoAllocatePickingListItemCPart
        /// called from OutboundDetail and OutboundPickingList
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("autoAllocate")]
        public async Task<ActionResult<bool>> AutoAllocate(AllocationDto data)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await pickingListService.AutoAllocate(data));
        }

        /// <summary>
        /// Allocate picking list
        /// original endpoint: AllocatePickingListItem
        /// </summary>
        /// <param name="pickingList"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("allocate")]
        public async Task<ActionResult<bool>> Allocate(IEnumerable<PickingListAllocateDto> pickingList)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await pickingListService.AllocatePickingListBatch(pickingList));
        }

        /// <summary>
        /// undo allocation for specific jobno/lineitem
        /// original endpoint:  UnAllocatedPickingListItem(string p_strJobNo, int p_intLineItem) (OutboundDetail > btnUndoAllocation_Click) 
        /// </summary>
        /// <param name="unallocationData">Contains JobNo (required), LineItem (required) and SeqNo (optional)</param>
        /// <returns></returns>
        [HttpPost]
        [Route("unAllocate")]
        public async Task<ActionResult<bool>> UnAllocate(IList<UndoAllocationDto> unallocationData)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await pickingListService.UnAllocateBatch(unallocationData));
        }

        private readonly IPickingListService pickingListService;
    }
}
