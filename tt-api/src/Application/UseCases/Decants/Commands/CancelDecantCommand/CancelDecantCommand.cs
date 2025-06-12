using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.Decants.Commands.CancelDecantCommand;

public class CancelDecantCommand : IRequest<Decant>
{
    public string JobNo { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class CancelDecantCommandHandler : IRequestHandler<CancelDecantCommand, Decant>
{
    private readonly IRepository repository;
    private IDateTime dateTimeService;

    public CancelDecantCommandHandler(IRepository repository, IDateTime dateTimeService)
    {
        this.repository = repository;
        this.dateTimeService = dateTimeService;
    }

    public async Task<Decant> Handle(CancelDecantCommand request, CancellationToken cancellationToken)
    {
        // check if user code exists 
        if (!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new UnknownUserCodeException();

        repository.BeginTransaction();
        var decant = await repository.Decant.GetDecant(request.JobNo);
        try
        {
            if (decant == null)
                throw new UnknownJobNoException();

            if (!decant.CanEdit)
                throw new IllegalDecantChangeException($"Decant with status 'Cancelled' or 'Completed' cannot be updated");

            foreach (var decantItem in decant.Items)
            {
                // STEP 1 - Update storage detail
                var pallet = repository.StorageDetails.GetPalletDetail(decantItem.SourcePalletId);
                if (pallet == null)
                    throw new UnknownPIDException();
                pallet.Status = StorageStatus.Putaway;
                await repository.StorageDetails.Update(pallet);

                // STEP 2 - Delete decant item (decant detail and decant pkg)
                await repository.Decant.DeleteDecantItem(decant.JobNo, decantItem);
            }
            // STEP 3 - Update decant (set status to cancel)
            decant.Cancel(request.UserCode, dateTimeService.Now);
            await repository.Decant.UpdateDecant(decant);

            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();

            // get updated decant from repository and send it back
            var updatedDecant = await repository.Decant.GetDecant(decant.JobNo);
            if (updatedDecant == null)
                throw new UnknownJobNoException();
            return updatedDecant;

        }
        catch (Exception)
        {
            repository.RollbackTransaction();
            throw;
        }
    }
}
