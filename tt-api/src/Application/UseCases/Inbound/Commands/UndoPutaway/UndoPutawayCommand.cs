using Application.Interfaces.Repositories;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.Inbound.Commands.UndoPutaway;

public class UndoPutawayCommand : IRequest
{
    public string[] Pids { get; set; } = default!;
}

public class UndoPutawayCommandHandler : IRequestHandler<UndoPutawayCommand>
{
    private readonly IRepository repository;

    public UndoPutawayCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Unit> Handle(UndoPutawayCommand r, CancellationToken cancellationToken)
    {
        repository.BeginTransaction();

        var pallets = repository.StorageDetails.GetPallets(r.Pids);
        if (pallets.Count != r.Pids.Distinct().Count())
        {
            var missing = r.Pids.Except(pallets.Select(p => p.Id)).Distinct();
            throw new ApplicationException($"Some of the selected PIDs could not be found. PIDs: {string.Join(',', missing)}.");
        }
        if (pallets.GroupBy(p => p.InboundJobNo).Count() > 1)
        {
            throw new ApplicationException($"Selected PIDs come from different inbounds.");
        }
        if (pallets.Any(p => p.Status != StorageStatus.Incoming))
        {
            throw new ApplicationException($"Some of the selected PIDs have incorrect status.");
        }
        if (pallets.Any(p => p.Location == string.Empty))
        {
            throw new ApplicationException($"Some of the selected PIDs have not been putaway yet.");
        }

        var inboundJobNo = pallets.First().InboundJobNo;
        var inboundStatus = repository.Inbounds.GetStatus(inboundJobNo);
        if (inboundStatus != InboundStatus.PartialPutaway)
        {
            throw new ApplicationException($"Inbound is not partially putaway.");
        }

        foreach (var pallet in pallets)
        {
            pallet.UndoPutaway();
            await repository.StorageDetails.Update(pallet);
        }
        await repository.SaveChangesAsync(cancellationToken);

        repository.Inbounds.UpdateStatusIfNoPutawayPallets(inboundJobNo);
        await repository.SaveChangesAsync(cancellationToken);

        repository.CommitTransaction();

        return Unit.Value;
    }
}
