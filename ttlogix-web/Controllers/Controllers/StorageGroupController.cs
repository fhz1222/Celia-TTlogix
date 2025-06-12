using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TT.Controllers.Extensions;
using TT.Core.QueryFilters;
using TT.Core.QueryResults;
using TT.Services.Interfaces;
using TT.Services.Models;
using TT.Services.Services.Utilities;

namespace TT.Controllers.Controllers
{
    [Authorize]
    [Route("api/storage_group")]
    [ApiController]
    public class StorageGroupController : TTLogixControllerBase
    {
        public StorageGroupController(IStorageGroupService storageGroupService)
            => this.storageGroupService = storageGroupService;

        /// <summary>
        /// Print Groups labels on selected printer
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("printLabels")]
        public async Task<ActionResult> PrintLabels([RequiredAsJsonError] PrintLabelsDto data)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await storageGroupService.PrintLabels(data.PID, Enum.Parse<ILabelFactory.LabelType>(data.Type), data.Printer, data.Copies));
        }

        /// <summary>
        /// Print Groups labels on selected printer
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("printPIDLabels")]
        public async Task<ActionResult> PrintPIDLabels([RequiredAsJsonError] PrintLabelsDto data)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await storageGroupService.PrintPIDLabels(data.PID, Enum.Parse<ILabelFactory.LabelType>(data.Type), data.Printer));
        }

        /// <summary>
        /// Get the list
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getGroups")]
        public async Task<ActionResult<StorageGroupListDto>> GetGroups([FromQuery] StorageGroupListQueryFilter queryFilter)
            => Ok(await storageGroupService.GetGroupList(User.GetWHSCode(), queryFilter));

        /// <summary>
        /// Create
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("createGroup")]
        public async Task<ActionResult<string[]>> CreateGroup([FromQuery(Name = "Qty")] int Qty = 1, [FromQuery(Name = "Prefix")] string Prefix = "Group")
            => Ok(await storageGroupService.CreateGroup(User.GetWHSCode(), Qty, Prefix));

        /// <summary>
        /// Delete group
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("{groupID}")]
        public async Task<ActionResult<string>> DeleteGroup([RequiredAsJsonError] string groupID)
        {
            await storageGroupService.DeleteGroup(groupID);
            return Ok(groupID);
        }

        /// <summary>
        /// Transform group
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("{groupID}/transform")]
        public async Task<ActionResult<string>> TransformGroup([RequiredAsJsonError] string groupID)
        {
            await storageGroupService.TransformGroup(groupID);
            return Ok(groupID);
        }

        /// <summary>
        /// PID info
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("details")]
        public async Task<ActionResult<IEnumerable<AllocatedStorageDetailSummaryQueryResult>>> GetDetails([RequiredAsJsonError] string groupId)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return Ok(await storageGroupService.GetStorageDetails(groupId));
        }

        /// <summary>
        /// Get PIDs labels
        /// </summary>
        /// <param name="gids"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getStorageLabelsForGIDs")]
        public async Task<ActionResult<IEnumerable<StorageLabelDto>>> getStorageLabelsForGIDs([FromBody] string[] gids)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await storageGroupService.GetStorageLabelsForGIDs(gids));
        }

        /// <summary>
        /// Get PIDs labels
        /// </summary>
        /// <param name="gids"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getGroupLabels")]
        public async Task<ActionResult<IEnumerable<GroupLabelDto>>> getGroupLabels([FromBody] string[] gids)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await storageGroupService.GetGroupLabels(gids));
        }

        private readonly IStorageGroupService storageGroupService;
    }
}
