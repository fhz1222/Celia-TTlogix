using Application.Exceptions;
using Application.Extensions;
using Application.Interfaces.Gateways;
using Application.Interfaces.Repositories;
using Application.UseCases.InvoiceRequest.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.InvoiceRequest.Commands.RequestNow;

public class RequestNowCommand : IRequest
{
    public string JobNo { get; set; } = default!;
    public string UserCode { get; set; } = null!;
}

public class RequestNowCommandHandler : IRequestHandler<RequestNowCommand>
{
    private readonly IRepository r;
    private readonly IInvoiceRequestRepository repository;
    private readonly ILogger<RequestNowCommandHandler> logger;
    private readonly IExcelWriter excelWriter;
    private readonly INotificationGateway notifications;

    public RequestNowCommandHandler(IRepository r, IInvoiceRequestRepository repository, ILogger<RequestNowCommandHandler> logger, IExcelWriter excelWriter, INotificationGateway notifications)
        => (this.r, this.repository, this.logger, this.excelWriter, this.notifications) = (r, repository, logger, excelWriter, notifications);

    public async Task<Unit> Handle(RequestNowCommand request, CancellationToken cancellationToken)
    {
        var flow = repository.GetFlow();
        if (flow == InvRequestFlow.None)
        {
            throw new ApplicationError("No invoice request flow assigned");
        }

        var jobNo = request.JobNo;
        var isOutbound = jobNo.StartsWith("OUT");
        var relevancy = -repository.GetRelevancyThreshold();

        var eligibleJobs = (isOutbound, flow) switch
        {
            (true, InvRequestFlow.Standard) => repository.GetOutboundsEligibleForStandardFlow(relevancy),
            (true, InvRequestFlow.CustomsClearance) => repository.GetOutboundsEligibleForCustomsClearanceFlow(relevancy),
            (false, InvRequestFlow.Standard) => repository.GetStockTransfersEligibleForStandardFlow(relevancy),
            (false, InvRequestFlow.CustomsClearance) => repository.GetStockTransfersEligibleForCustomsClearanceFlow(relevancy),
            _ => throw new ApplicationError()
        };

        var jobResults = eligibleJobs.Where(i => i.JobNo == jobNo).ToList();
        if (jobResults.None())
        {
            throw new ApplicationError($"Job {jobNo} is not eligible for issuing invoice request");
        }
        if (jobResults.Count > 1)
        {
            throw new ApplicationError("Job contains goods from different suppliers");
        }

        var job = jobResults.Single();
        var productLines = await repository.GetProductsForInvoiceRequest(jobNo);

        try
        {
            await SendEmail(job, productLines);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error");
            throw new ApplicationError("Internal error. Unable to send invoice request to supplier");
        }

        // save the request
        await repository.CreateInvoiceRequest(job.CustomerCode, job.SupplierId, jobNo, job.SupplierRefNo, request.UserCode, productLines);
        await r.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private async Task SendEmail(JobForSupplier job, List<ProductLineDto> lines)
    {
        var supplierDto = repository.GetCustomerSupplierData(job.CustomerCode, job.SupplierId);
        var excelStream = excelWriter.GetInvoiceRequestExcel(supplierDto, job.JobNo, job.SupplierRefNo, lines);
        await notifications.EmailAboutInvoiceRequest(supplierDto, job, lines, excelStream);
    }
}
