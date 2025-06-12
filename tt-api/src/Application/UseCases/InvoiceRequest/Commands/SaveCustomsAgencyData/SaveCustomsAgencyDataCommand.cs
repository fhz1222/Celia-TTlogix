using Application.Exceptions;
using Application.Extensions;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.InvoiceRequest.Commands.SaveCustomsAgencyData;

public class SaveCustomsAgencyDataCommand : IRequest
{
    public int BatchId { get; set; } = default!;
    public int TruckDepartureHour { get; set; } = default!;
    public string? Comment { get; set; }
}

public class SaveCustomsAgencyDataCommandHandler : IRequestHandler<SaveCustomsAgencyDataCommand>
{
    private readonly IRepository repository;

    public SaveCustomsAgencyDataCommandHandler(IRepository repository) => this.repository = repository;

    public async Task<Unit> Handle(SaveCustomsAgencyDataCommand request, CancellationToken cancellationToken)
    {
        repository.BeginTransaction();

        _ = await repository.InvoiceRequests.GetBatch(request.BatchId)
            ?? throw new ApplicationError($"Batch could not be found. Batch id {request.BatchId}.");

        var allowedRange = Enumerable.Range(9, 8);
        if (allowedRange.DoesNotContain(request.TruckDepartureHour))
        {
            throw new ApplicationError("Truck departure hour is outside of allowed range 9:00-17:00.");
        }

        await repository.InvoiceRequests.UpsertCustomsAgencyData(request.BatchId, request.TruckDepartureHour, request.Comment);

        await repository.SaveChangesAsync(cancellationToken);
        repository.CommitTransaction();

        return Unit.Value;
    }
}
