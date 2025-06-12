using Application.Exceptions;
using Application.Interfaces.Gateways;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.InvoiceRequest.Commands.RejectBatch;

public class RejectBatchCommand : IRequest
{
    public int BatchId { get; set; } = default!;
    public string UserCode { get; set; } = default!;
}

public class RejectBatchCommandHandler : IRequestHandler<RejectBatchCommand>
{
    private readonly IRepository repository;
    private readonly IDateTime dateTime;
    private readonly INotificationGateway notifications;

    public RejectBatchCommandHandler(IRepository repository, IDateTime dateTime, INotificationGateway notifications)
    {
        this.repository = repository;
        this.dateTime = dateTime;
        this.notifications = notifications;
    }

    public async Task<Unit> Handle(RejectBatchCommand request, CancellationToken cancellationToken)
    {
        repository.BeginTransaction();

        var batch = await repository.InvoiceRequests.GetBatch(request.BatchId)
            ?? throw new ApplicationError($"Batch not found. Batch id {request.BatchId}.");

        batch.Reject(dateTime.Now, request.UserCode);
        await repository.InvoiceRequests.Update(batch);
        await repository.InvoiceRequests.DeleteInvoiceFiles(batch.Id);

        await repository.SaveChangesAsync(cancellationToken);
        repository.CommitTransaction();

        await NotifyAboutRejection(batch);

        return Unit.Value;
    }

    private async Task NotifyAboutRejection(InvoiceBatch batch)
    {
        var supplier = repository.InvoiceRequests.GetCustomerSupplierData(batch.FactoryId, batch.SupplierId);
        var requests = await repository.InvoiceRequests.GetRequests(batch.Id);
        var dockets = requests.Select(r => r.SupplierRefNo).ToList();
        RunInNewThread(async () => await notifications.EmailAboutRejectedInvoiceBatch(supplier, dockets));
    }

    private void RunInNewThread(Func<Task?> action) => Task.Run(action).ConfigureAwait(false);
}
