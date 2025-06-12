using Application.Common.Models;
using Application.UseCases.ILogIntegration.Commands.GenerateILogStockSyncFile;
using Application.UseCases.Storage.Queries.GetILogInboundPalletsWithTypeQuery;
using Application.UseCases.Storage.Queries.GetPalletsForILogStockDiscrepancyReport;
using Application.UseCases.Storage.Queries.GetPidsInILog;
using Application.UseCases.Storage.Queries.GetStorageDetailItems;
using Application.UseCases.StorageDetails;
using Application.UseCases.StorageDetails.Queries.GetStorageDetailItems;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Presentation.Common;

namespace Presentation.Controllers;

/// <summary>
/// StorageDetailController provides method to get the storage detail list
/// </summary>
public partial class StorageDetailController : ApiControllerBase
{
    private readonly IMapper mapper;
    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="mapper"></param>
    public StorageDetailController(IMapper mapper)
    {
        this.mapper = mapper;
    }

    /// <summary>
    /// Gets storage detail items filtered by specified filters
    /// </summary>
    [HttpGet]
    [Produces("application/json")]
    public async Task<StorageDetailPaginatedList> GetStorageDetailItems([FromQuery] StorageDetailItemParameters filter, string whsCode)
    {
        var gridFilter = mapper.Map<StorageDetailItemDtoFilter>(filter);
        gridFilter.WhsCode = whsCode;
        return await Mediator.Send(new GetStorageDetailItemsQuery()
        {
            Pagination = filter.Pagination,
            Filter = gridFilter,
            OrderByExpression = filter.Sorting?.By,
            OrderByDescending = filter.Sorting?.Descending ?? false
        }); 
    }

    /// <summary>
    /// Gets the storage detail list for outbound line 
    /// Returns paginated list of available items to be picked from storage
    /// </summary>
    [HttpGet]
    [Produces("application/json")]
    [Route("getStorageDetailWithPartsInfoList")]
    public async Task<PaginatedList<StorageDetailItemWithPartInfoDto>> GetStorageDetailWithPartsInfoList([FromQuery] StorageDetailItemWithPartInfoParameters filter, string whsCode)
    {
        var gridFilter = mapper.Map<StorageDetailItemWithPartInfoDtoFilter>(filter);
        gridFilter.WhsCode = whsCode;
        return await Mediator.Send(new GetStorageDetailItemsWithPartInfoQuery()
        {
            Pagination = filter.Pagination,
            Filter = gridFilter,
            OrderByExpression = filter.Sorting?.By,
            OrderByDescending = filter.Sorting?.Descending ?? false
        });
    }

    /// <summary>
    /// Gets the pallet list with type
    /// </summary>
    [HttpGet]
    [Produces("application/json")]
    [Route("getILogInboundPalletWithType")]
    public async Task<IEnumerable<ILogInboundPalletWithTypeDto>> GetILogInboundPalletsWithType([FromQuery] ILogInboundPalletsWithTypeDtoFilter filter)
    {
        return await Mediator.Send(new GetILogInboundPalletsWithTypeQuery()
        {
            Filter = filter,
        });
    }

    /// <summary>
    /// Gets the pallet list for ilog connect stock discrepancy report (not dispatched in iLogStorage location with Qty > 0)
    /// </summary>
    [HttpGet]
    [Produces("application/json")]
    [Route("getPalletsForILogStockDiscrepancyReport")]
    public async Task<IEnumerable<StockDiscrepancyReportPalletDto>> GetPalletsForILogStockDiscrepancyReport([FromQuery] StockDiscrepancyReportPalletDtoFilter filter)
    {
        return await Mediator.Send(new GetPalletsForILogStockDiscrepancyReportQuery()
        {
            Filter = filter
        });
    }

    /// <summary>
    /// Gets the pallet list (a csv file) for ilog initial stock synchronization (not dispatched in iLogStorage location with Qty > 0)
    /// </summary>
    [HttpGet]
    [Produces("application/json")]
    [Route("getILogStockSynchronizationCSV")]
    public async Task<FileStreamResult> GetILogStockSynchronizationCSV()
    {
        var namedStream = await Mediator.Send(new GenerateILogStockSyncFileCommand());

        var fileStream = new FileStreamResult(namedStream.Stream, MediaTypeHeaderValue.Parse("text/plain")) { FileDownloadName = namedStream.Name };
        return await Task.FromResult(fileStream);
    }

    [HttpGet("getPidsInILog")]
    public async Task<List<string>> GetPidsInILog([FromQuery] string[] pids)
    {
        return await Mediator.Send(new GetPidsInILogQuery() { Pids = pids });
    }
}