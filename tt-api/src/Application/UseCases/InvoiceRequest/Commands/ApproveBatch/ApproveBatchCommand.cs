using Application.Exceptions;
using Application.Interfaces.Gateways;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Application.UseCases.InvoiceRequest.Queries;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.InvoiceRequest.Commands.ApproveBatch;

public class ApproveBatchCommand : IRequest
{
    public int BatchId { get; set; } = default!;
    public string UserCode { get; set; } = default!;
}

public class ApproveBatchCommandHandler : IRequestHandler<ApproveBatchCommand>
{
    private readonly IRepository repository;
    private readonly IDateTime dateTime;
    private readonly INotificationGateway notifications;
    private readonly IExcelWriter excelWriter;

    public ApproveBatchCommandHandler(IRepository repository, IDateTime dateTime, INotificationGateway notifications, IExcelWriter excelWriter)
    {
        this.repository = repository;
        this.dateTime = dateTime;
        this.notifications = notifications;
        this.excelWriter = excelWriter;
    }

    public async Task<Unit> Handle(ApproveBatchCommand request, CancellationToken cancellationToken)
    {
        var flow = repository.InvoiceRequests.GetFlow();

        repository.BeginTransaction();

        var batch = await repository.InvoiceRequests.GetBatch(request.BatchId)
            ?? throw new ApplicationError($"Batch not found. Batch id {request.BatchId}.");

        if (batch.IsApproved)
        {
            throw new ApplicationError($"Invoice batch {batch.BatchNumber} was already approved. Batch id {batch.Id}.");
        }

        var requests = await repository.InvoiceRequests.GetRequests(batch.Id);
        if (requests.Any(r => r.IsCompleted))
        {
            throw new ApplicationError($"Invoice request(s) from this batch have already been approved. Batch id {batch.Id}.");
        }

        var customsInfo = repository.InvoiceRequests.GetCustomsInfo(batch.Id);
        if (flow == InvRequestFlow.CustomsClearance && customsInfo?.departureHour is null)
        {
            throw new ApplicationError($"Truck departure time was not provided. Batch id {batch.Id}.");
        }

        batch.Approve(dateTime.Now, request.UserCode, requests);

        await repository.InvoiceRequests.Update(batch);
        foreach (var r in requests)
        {
            await repository.InvoiceRequests.Update(r);
        }

        var jobNumbers = requests.Select(r => r.JobNo).ToList();
        await repository.InvoiceRequests.UpdateJobCommercialInvNumber(batch.Id, jobNumbers);

        await repository.SaveChangesAsync(cancellationToken);
        repository.CommitTransaction();

        jobNumbers.ForEach(jobNo => repository.InvoiceRequests.AddForCustomsAgencyIntegration(jobNo));
        await repository.SaveChangesAsync(cancellationToken);

        await SendEmails(flow, batch, requests, customsInfo?.departureHour, customsInfo?.comment);

        return Unit.Value;
    }

    private async Task SendEmails(InvRequestFlow flow, InvoiceBatch batch, List<Domain.Entities.InvoiceRequest> requests, string? hour, string? comment)
    {
        var supplier = repository.InvoiceRequests.GetCustomerSupplierData(batch.FactoryId, batch.SupplierId);
        var sourceInvoiceFiles = await repository.InvoiceRequests.GetInvoiceFiles(batch.Id);

        if (flow == InvRequestFlow.Standard)
        {
            var dockets = requests.Select(r => r.SupplierRefNo).ToList();
            RunInNewThread(async () => await notifications.EmailAboutApprovedInvoiceBatchStandardFlow(supplier, dockets, sourceInvoiceFiles));
        }
        else
        {
            var dataForEmails = new List<(Domain.Entities.InvoiceRequest, JobForSupplier, List<ProductLineDto>)>();
            foreach (var r in requests)
            {
                var job = repository.InvoiceRequests.GetJob(r.JobNo);
                var lines = await repository.InvoiceRequests.GetProductsForInvoiceRequest(r.JobNo);
                dataForEmails.Add((r, job, lines));
            }

            RunInNewThread(async () =>
            {
                foreach (var (r, job, lines) in dataForEmails)
                {
                    var excel = excelWriter.GetInvoiceRequestExcel(supplier, r.JobNo, r.SupplierRefNo, lines);
                    var invoiceFiles = sourceInvoiceFiles.Select(f => f.Clone()).ToList();
                    await notifications.EmailAboutApprovedInvoiceRequestCustomsFlow(supplier, job, lines, excel, invoiceFiles, hour!, comment);
                }
            });
        }
    }

    private void RunInNewThread(Func<Task?> action) => Task.Run(action).ConfigureAwait(false);
}