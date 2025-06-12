using Application.Common.Models;
using Application.UseCases.InvoiceRequest.Commands.ApproveBatch;
using Application.UseCases.InvoiceRequest.Commands.Block;
using Application.UseCases.InvoiceRequest.Commands.RejectBatch;
using Application.UseCases.InvoiceRequest.Commands.RequestAll;
using Application.UseCases.InvoiceRequest.Commands.RequestNow;
using Application.UseCases.InvoiceRequest.Commands.SaveCustomsAgencyData;
using Application.UseCases.InvoiceRequest.Commands.Unblock;
using Application.UseCases.InvoiceRequest.Commands.UploadBatch;
using Application.UseCases.InvoiceRequest.Commands.ValidatePrice;
using Application.UseCases.InvoiceRequest.Queries;
using Application.UseCases.InvoiceRequest.Queries.GetBatches;
using Application.UseCases.InvoiceRequest.Queries.GetBatchesReport;
using Application.UseCases.InvoiceRequest.Queries.GetFactories;
using Application.UseCases.InvoiceRequest.Queries.GetFlow;
using Application.UseCases.InvoiceRequest.Queries.GetInvoiceFile;
using Application.UseCases.InvoiceRequest.Queries.GetJobStatus;
using Application.UseCases.InvoiceRequest.Queries.GetNotInvoicedReport;
using Application.UseCases.InvoiceRequest.Queries.GetOpenRequests;
using Application.UseCases.InvoiceRequest.Queries.GetSupplierFactories;
using Application.UseCases.InvoiceRequest.Queries.ShouldValidatePrice;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using Presentation.Common;
using Presentation.Configuration;

namespace Presentation.Controllers;

/// <summary>
/// InvoiceRequestController provides methods to manage invoice requests
/// </summary>
public class InvoiceRequestController : ApiControllerBase
{
    private readonly IMapper mapper;
    private readonly IFeatureManager manager;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="manager"></param>
    public InvoiceRequestController(IMapper mapper, IFeatureManager manager)
    {
        this.mapper = mapper;
        this.manager = manager;
    }

    [HttpGet("isActive")]
    public async Task<bool> IsActive()
        => await manager.IsEnabledAsync(FeatureFlags.InvoiceRequest);

    /// <summary>
    /// Returns currently active flow in hub
    /// </summary>
    /// <returns>Active flow: Standard, CustomsClearance or None</returns>
    [FeatureGate(FeatureFlags.InvoiceRequest)]
    [HttpGet("flow")]
    public async Task<string> GetFlow()
        => await Mediator.Send(new GetFlowQuery());

    /// <summary>
    /// Gets invoice batches
    /// </summary>
    /// <param name="parameters">Parameters and filters to apply</param>
    /// <param name="whsCode">Warehouse code</param>
    /// <returns>Paginated list of batches</returns>
    [FeatureGate(FeatureFlags.InvoiceRequest)]
    [HttpGet("batches")]
    public async Task<PaginatedList<InvoiceBatchDto>> GetBatches([FromQuery] GetBatchesRequestParameters parameters, [FromQuery] string? whsCode)
    {
        var filter = mapper.Map<GetBatchesQueryFilter>(parameters);
        filter.WhsCode = whsCode;
        return await Mediator.Send(new GetBatchesQuery()
        {
            Filter = filter,
            Pagination = parameters.Pagination
        });
    }

    /// <summary>
    /// Gets invoice file
    /// </summary>
    /// <param name="invoiceFileId">Invoice file identifier</param>
    /// <returns>File stream</returns>
    [FeatureGate(FeatureFlags.InvoiceRequest)]
    [HttpGet("file/{invoiceFileId}")]
    public async Task<FileStreamResult> GetFile([FromRoute] int invoiceFileId)
    {
        var (fileStream, fileName) = await Mediator.Send(new GetInvoiceFileQuery() { InvoiceFileId = invoiceFileId });
        return new FileStreamResult(fileStream, fileName.EndsWith(".pdf") ? "application/pdf" : "application/octet-stream")
        {
            FileDownloadName = fileName
        };
    }

    // FOR TTLOGIX

    /// <summary>
    ///  Gets factories supplied by warehouse
    /// </summary>
    /// <param name="whsCode">Warehouse code</param>
    /// <returns>List of factories</returns>
    [FeatureGate(FeatureFlags.InvoiceRequest)]
    [HttpGet("factories")]
    public async Task<List<FactoryDto>> GetWarehouseFactories([FromQuery] string whsCode)
        => await Mediator.Send(new GetWarehouseFactoriesQuery() { WarehouseCode = whsCode });

    /// <summary>
    /// Gets invoice request flags for TTLogix job
    /// </summary>
    /// <param name="jobNo">Outbound / stock transfer job number</param>
    /// <returns>Object with flags</returns>
    [FeatureGate(FeatureFlags.InvoiceRequest)]
    [HttpGet("status/{jobNo}")]
    public async Task<JobStatusDto> GetStatus([FromRoute] string jobNo)
        => await Mediator.Send(new GetJobStatusQuery() { JobNo = jobNo });

    /// <summary>
    /// Blocks invoice request creation for outbound / stock transfer
    /// </summary>
    /// <param name="jobNo">Outbound / stock transfer job number to block</param>
    /// <param name="userCode">User requesting blocking</param>
    /// <returns></returns>
    [FeatureGate(FeatureFlags.InvoiceRequest)]
    [HttpPost("block/{jobNo}")]
    public async Task Block([FromRoute] string jobNo, [FromQuery] string userCode)
        => await Mediator.Send(new BlockCommand() { JobNo = jobNo, UserCode = userCode });

    /// <summary>
    /// Unblocks invoice request creation for outbound / stock transfer
    /// </summary>
    /// <param name="jobNo">Outbound / stock transfer job number to unblock</param>
    /// <returns></returns>
    [FeatureGate(FeatureFlags.InvoiceRequest)]
    [HttpPost("unblock/{jobNo}")]
    public async Task Unblock([FromRoute] string jobNo)
        => await Mediator.Send(new UnblockCommand() { JobNo = jobNo });

    /// <summary>
    /// Creates invoice requests for all eligible TTLogix jobs
    /// </summary>
    /// <returns></returns>
    [FeatureGate(FeatureFlags.InvoiceRequest)]
    [HttpPost("requestAll")]
    public async Task RequestAll()
        => await Mediator.Send(new RequestAllCommand());

    /// <summary>
    /// Creates invoice request for TTLogix job 
    /// </summary>
    /// <param name="jobNo">Eligible outbound / stock transfer job number</param>
    /// <param name="userCode">User requesting invoice request creation</param>
    /// <returns></returns>
    [FeatureGate(FeatureFlags.InvoiceRequest)]
    [HttpPost("requestNow/{jobNo}")]
    public async Task RequestNow([FromRoute] string jobNo, [FromQuery] string userCode)
        => await Mediator.Send(new RequestNowCommand() { JobNo = jobNo, UserCode = userCode });

    /// <summary>
    /// Approves invoice batch
    /// </summary>
    /// <param name="batchId">Invoice batch ID to approve</param>
    /// <param name="userCode">User requesting approval</param>
    /// <returns></returns>
    [FeatureGate(FeatureFlags.InvoiceRequest)]
    [HttpPost("approve/{batchId}")]
    public async Task Approve([FromRoute] int batchId, [FromQuery] string userCode)
        => await Mediator.Send(new ApproveBatchCommand() { BatchId = batchId, UserCode = userCode });

    /// <summary>
    /// Rejects invoice batch
    /// </summary>
    /// <param name="batchId">Invoice batch ID to reject</param>
    /// <param name="userCode">User requesting rejection</param>
    /// <returns></returns>
    [FeatureGate(FeatureFlags.InvoiceRequest)]
    [HttpPost("reject/{batchId}")]
    public async Task Reject([FromRoute] int batchId, [FromQuery] string userCode)
        => await Mediator.Send(new RejectBatchCommand() { BatchId = batchId, UserCode = userCode });

    /// <summary>
    /// Updates invoice batch data for customs agency
    /// </summary>
    /// <param name="batchId">Batch ID to update</param>
    /// <param name="request">Object with data for customs agency: hour and comment</param>
    /// <returns></returns>
    [FeatureGate(FeatureFlags.InvoiceRequest)]
    [HttpPost("saveDataForCustomsAgency/{batchId}")]
    public async Task SaveDataForCustomsAgency([FromRoute] int batchId, [FromBody] CustomsAgencyDataDto request)
        => await Mediator.Send(new SaveCustomsAgencyDataCommand { BatchId = batchId, TruckDepartureHour = request.Hour, Comment = request.Comment });

    // FOR VMI

    /// <summary>
    /// Gets factories supplied by supplier
    /// </summary>
    /// <param name="supplierId">Supplier code</param>
    /// <returns>List of factories</returns>
    [FeatureGate(FeatureFlags.InvoiceRequest)]
    [HttpGet("factories/{supplierId}")]
    public async Task<List<FactoryDto>> GetFactories([FromRoute] string supplierId)
        => await Mediator.Send(new GetSupplierFactoriesQuery() { SupplierId = supplierId });

    /// <summary>
    /// Gets TTLogix jobs with open invoice request
    /// </summary>
    /// <param name="factoryId">Factory code</param>
    /// <param name="supplierId">Supplier code</param>
    /// <returns>List of TTLogix jobs </returns>
    [FeatureGate(FeatureFlags.InvoiceRequest)]
    [HttpGet("openRequests")]
    public async Task<List<JobDto>> GetOpenRequests([FromQuery] string factoryId, [FromQuery] string supplierId)
        => await Mediator.Send(new GetOpenRequestsQuery() { FactoryId = factoryId, SupplierId = supplierId });

    /// <summary>
    /// Returns whether price validation has to be performed before invoice batch upload
    /// </summary>
    /// <returns></returns>
    [FeatureGate(FeatureFlags.InvoiceRequest)]
    [HttpGet("shouldValidatePrice")]
    public async Task<bool> ShouldValidatePrice()
        => await Mediator.Send(new ShouldValidatePriceQuery() { });

    /// <summary>
    /// Performs invoice batch price validation
    /// </summary>
    /// <param name="request">Invoice batch object for validation</param>
    /// <returns>Validation result object</returns>
    [FeatureGate(FeatureFlags.InvoiceRequest)]
    [HttpPost("validatePrice")]
    public async Task<ValidationResultDto> ValidatePrice([FromBody] ValidatePriceRequestDto request)
        => await Mediator.Send(mapper.Map<ValidatePriceCommand>(request));

    /// <summary>
    /// Saves new invoice batch in the system
    /// </summary>
    /// <param name="request">Invoice batch object to save</param>
    /// <returns>Created batch identifier</returns>
    [FeatureGate(FeatureFlags.InvoiceRequest)]
    [HttpPost("uploadBatch")]
    public async Task<string> UploadBatch([FromBody] UploadBatchRequestDto request)
        => await Mediator.Send(mapper.Map<UploadBatchCommand>(request));

    /// <summary>
    /// Generates Excel with open invoice requests of supplier
    /// </summary>
    /// <param name="factoryId">Factory code</param>
    /// <param name="supplierId">Supplier code</param>
    /// <returns>Excel file stream</returns>
    [FeatureGate(FeatureFlags.InvoiceRequest)]
    [HttpGet("notInvoicedReport")]
    public async Task<FileStreamResult> GetNotInvoicedExcelReport([FromQuery] string factoryId, [FromQuery] string supplierId)
    {
        var reportStream = await Mediator.Send(new GetNotInvoicedReportQuery() { FactoryId = factoryId, SupplierId = supplierId });
        return new FileStreamResult(reportStream, "application/octet-stream")
        {
            FileDownloadName = $"Not_Invoiced_{DateTime.Now:ddMMMyyyy}.xlsx"
        };
    }

    /// <summary>
    /// Generates Excel with supplier invoice batches
    /// </summary>
    /// <param name="parameters">Parameters and filters to apply</param>
    /// <returns>Excel file stream</returns>
    [FeatureGate(FeatureFlags.InvoiceRequest)]
    [HttpGet("batchesReport")]
    public async Task<FileStreamResult> GetBatchesToExcel([FromQuery] GetBatchesRequestParameters parameters)
    {
        // To KS: if there is "load more" in VMI then pagination info need to be passed correctly:
        // (page number = 1 and page size = {number of records on the screen})

        var filter = mapper.Map<GetBatchesQueryFilter>(parameters);
        var reportStream = await Mediator.Send(new GetBatchesReportQuery()
        {
            Filter = filter,
            Pagination = parameters.Pagination
        });

        return new FileStreamResult(reportStream, "application/octet-stream")
        {
            FileDownloadName = $"Invoice_Batch_{DateTime.Now:ddMMMyyyy}.xlsx"
        };
    }
}
