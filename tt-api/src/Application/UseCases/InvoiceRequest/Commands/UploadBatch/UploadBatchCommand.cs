using Application.Exceptions;
using Application.Extensions;
using Application.Interfaces.Gateways;
using Application.Interfaces.Repositories;
using Application.UseCases.InvoiceRequest.Commands.ApproveBatch;
using Application.UseCases.InvoiceRequest.Queries;
using Application.UseCases.InvoiceRequest.Queries.ShouldValidatePrice;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.InvoiceRequest.Commands.UploadBatch;

public class UploadBatchCommand : IRequest<string>
{
    public string SupplierId { get; set; } = default!;
    public string FactoryId { get; set; } = default!;
    public int? PriceValidationId { get; set; } = default!;
    public List<int> RequestIds { get; set; } = default!;
    public List<UploadInvoiceDto> Invoices { get; set; } = default!;
    public string LoginId { get; set; } = default!;
}

public class UploadBatchCommandHandler : IRequestHandler<UploadBatchCommand, string>
{
    private readonly IRepository repository;
    private readonly IMediator mediator;
    private readonly INotificationGateway notifications;

    public UploadBatchCommandHandler(IRepository repository, IMediator mediator, INotificationGateway notifications)
    {
        this.repository = repository;
        this.mediator = mediator;
        this.notifications = notifications;
    }

    public async Task<string> Handle(UploadBatchCommand r, CancellationToken cancellationToken)
    {
        var flow = repository.InvoiceRequests.GetFlow();

        repository.BeginTransaction();

        var requests = await repository.InvoiceRequests.GetRequests(r.RequestIds);

        // validation
        if (requests.Any(s => s.SupplierId != r.SupplierId))
        {
            throw new ApplicationError("Selected invoice requests are for a different supplier");
        }
        if (requests.Any(s => s.FactoryId != r.FactoryId))
        {
            throw new ApplicationError("Selected invoice requests are for a different factory");
        }
        if (r.RequestIds.Count != requests.Count)
        {
            throw new ApplicationError("Some invoice requests could not be found");
        }
        if (r.RequestIds.None())
        {
            throw new ApplicationError("No invoice requests provided");
        }
        if (r.Invoices.None())
        {
            throw new ApplicationError("No invoices provided");
        }

        var openRequestIds = await repository.InvoiceRequests.GetOpenRequests(r.FactoryId, r.SupplierId);
        var openRequests = openRequestIds.ToDictionary(r => r.RequestId);

        var notOpenRequests = r.RequestIds.Where(r => !openRequests.ContainsKey(r)).ToList();
        if (notOpenRequests.Any())
        {
            throw new ApplicationError($"Some of the selected invoice requests are no longer open");
        }

        var withValidation = await mediator.Send(new ShouldValidatePriceQuery(), cancellationToken);
        var validation = await GetPriceValidation(withValidation, r);

        // creation
        var batchNo = await repository.InvoiceRequests.GetNextBatchNumber(r.FactoryId, r.SupplierId);
        var currency = validation?.Currency ?? string.Empty;
        var batchId = await repository.InvoiceRequests.CreateBatch(batchNo, r.SupplierId, r.FactoryId, r.LoginId, r.RequestIds, r.Invoices, currency);

        await repository.SaveChangesAsync(cancellationToken);
        repository.CommitTransaction();

        // email planner
        var supplier = repository.InvoiceRequests.GetCustomerSupplierData(r.FactoryId, r.SupplierId);
        var docketDetails = requests.Select(r => (r.SupplierRefNo, r.FactoryId)).ToList();
        if (flow == InvRequestFlow.CustomsClearance)
        {
            NotifyAboutNewBatch(supplier, docketDetails);
        }

        // post-upload autoapproval
        var withAutoApproval = flow == InvRequestFlow.Standard;
        if (withAutoApproval)
        {
            var isApproved = !withValidation || (validation is { } && validation.Success);
            if (isApproved)
            {
                await mediator.Send(new ApproveBatchCommand() { BatchId = batchId, UserCode = "system" }, cancellationToken);
            }

            if (validation is { })
            {
                if (validation.HasFailed)
                {
                    var dds = docketDetails.Select(x => x.SupplierRefNo).ToList();
                    NotifyAboutPriceMismatch(supplier, dds, validation);
                }
                repository.InvoiceRequests.DeletePriceValidation(validation.Id);
                await repository.SaveChangesAsync(cancellationToken);
            }
        }

        return batchNo;
    }

    private async Task<PriceValidation?> GetPriceValidation(bool withValidation, UploadBatchCommand r)
    {
        if (!withValidation) { return null; }

        if (r.PriceValidationId is null)
        {
            throw new ApplicationError("Price validation is required");
        }

        var validation = await repository.InvoiceRequests.GetPriceValidation(r.PriceValidationId.Value);
        CheckPriceValidation(r, validation);
        return validation;
    }

    private void CheckPriceValidation(UploadBatchCommand r, PriceValidation? validation)
    {
        if (validation is null)
        {
            throw new ApplicationError("Price validation was not performed");
        }
        if (r.Invoices.Sum(i => i.Value) != validation.InvoiceTotalValue)
        {
            throw new ApplicationError("Invalid price validation");
        }

        var validationRequestIds = repository.InvoiceRequests.GetValidationRequestIds(validation.Id);
        var sequenceEqual = validationRequestIds.OrderBy(r => r).SequenceEqual(r.RequestIds.OrderBy(r => r));
        if (!sequenceEqual)
        {
            throw new ApplicationError("Price validation for these invoice requests was not performed but it's required");
        }
    }

    private void NotifyAboutNewBatch(CustomerSupplierDto supplier, List<(string dd, string factory)> details)
    {
        RunInNewThread(async () => await notifications.EmailAboutInvoiceBatchUploadCustomsFlow(supplier.CompanyName, details));
    }

    private void NotifyAboutPriceMismatch(CustomerSupplierDto supplier, List<string> dockets, PriceValidation validation)
    {
        var ttlogixPrice = $"{validation.TtlogixTotalValue}{validation.Currency}";
        var invoicePrice = $"{validation.InvoiceTotalValue}{validation.Currency}";
        RunInNewThread(async () => await notifications.EmailAboutBatchWithPriceValidationError(supplier, dockets, ttlogixPrice, invoicePrice));
    }

    private void RunInNewThread(Func<Task?> action) => Task.Run(action).ConfigureAwait(false);
}
