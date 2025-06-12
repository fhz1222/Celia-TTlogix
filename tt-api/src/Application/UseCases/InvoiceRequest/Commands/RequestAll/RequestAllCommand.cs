using Application.Exceptions;
using Application.Extensions;
using Application.Interfaces.Gateways;
using Application.Interfaces.Repositories;
using Application.UseCases.InvoiceRequest.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.InvoiceRequest.Commands.RequestAll;

public class RequestAllCommand : IRequest
{

}

public class RequestAllCommandHandler : IRequestHandler<RequestAllCommand>
{
    private readonly IRepository r;
    private readonly IInvoiceRequestRepository repository;
    private readonly ILogger<RequestAllCommandHandler> logger;
    private readonly IExcelWriter excelWriter;
    private readonly INotificationGateway notifications;

    public RequestAllCommandHandler(IRepository r, IInvoiceRequestRepository repository, ILogger<RequestAllCommandHandler> logger, IExcelWriter excelWriter, INotificationGateway notifications)
    {
        this.repository = repository;
        this.r = r;
        this.logger = logger;
        this.excelWriter = excelWriter;
        this.notifications = notifications;
    }

    public async Task<Unit> Handle(RequestAllCommand request, CancellationToken cancellationToken)
    {
        var flow = repository.GetFlow();
        if (flow == InvRequestFlow.None)
        {
            return await Task.FromResult(Unit.Value);
        }

        var relevancy = -repository.GetRelevancyThreshold();

        var outbounds = flow == InvRequestFlow.Standard
            ? repository.GetOutboundsEligibleForStandardFlow(relevancy)
            : repository.GetOutboundsEligibleForCustomsClearanceFlow(relevancy);
        var stockTransfers = flow == InvRequestFlow.Standard
            ? repository.GetStockTransfersEligibleForStandardFlow(relevancy)
            : repository.GetStockTransfersEligibleForCustomsClearanceFlow(relevancy);
        var jobs = outbounds.Union(stockTransfers).ToList();

        var multiSupplier = jobs
            .GroupBy(j => j.JobNo)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (multiSupplier.Any())
        {
            logger.LogError("There are jobs containing goods from different suppliers - requests not sent. Jobs: {j}", string.Join(",", multiSupplier));
        }

        var validJobs = jobs.Where(j => multiSupplier.DoesNotContain(j.JobNo)).ToList();

        var jobNumbers = validJobs.Select(j => j.JobNo).ToArray();
        var productLines = (await repository.GetProductsForInvoiceRequest(jobNumbers)).ToLookup(j => j.JobNo);

        foreach (var j in validJobs)
        {
            var products = productLines[j.JobNo].ToList();

            try
            {
                await SendEmail(j, products);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error");
                throw new ApplicationError("Internal error. Unable to send invoice request to supplier");
            }

            await repository.CreateInvoiceRequest(j.CustomerCode, j.SupplierId, j.JobNo, j.SupplierRefNo, "system", products);
        }
        await r.SaveChangesAsync(cancellationToken);

        return await Task.FromResult(Unit.Value);
    }

    private async Task SendEmail(JobForSupplier job, List<ProductLineDto> lines)
    {
        var supplierDto = repository.GetCustomerSupplierData(job.CustomerCode, job.SupplierId);
        var excelStream = excelWriter.GetInvoiceRequestExcel(supplierDto, job.JobNo, job.SupplierRefNo, lines);
        await notifications.EmailAboutInvoiceRequest(supplierDto, job, lines, excelStream);
    }
}
