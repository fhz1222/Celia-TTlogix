using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using MediatR;

namespace Application.UseCases.PalletTransferRequests.Commands.AddNewPalletTransferRequestCommand;

public class CompletePalletTransferRequestCommand : IRequest
{
    public string JobNo { get; set; } = null!;
    public string Pid { get; set; } = null!;
}

public class CompletePalletTransferRequestCommandHandler : IRequestHandler<CompletePalletTransferRequestCommand>
{
    private readonly IRepository repository;
    private readonly IDateTime dateTime;

    public CompletePalletTransferRequestCommandHandler(IRepository repository, IDateTime dateTime)
    {
        this.repository = repository;
        this.dateTime = dateTime;
    }

    public async Task<Unit> Handle(CompletePalletTransferRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            repository.BeginTransaction();

            var ptr = await repository.PalletTransferRequests.Get(request.JobNo) ?? throw new UnknownJobNoException();
            if (ptr.IsCompleted) 
            { 
                throw new JobAlreadyCompletedException(); 
            }
            if (ptr.PID != request.Pid)
            {
                throw new UnknownPIDException($"Different pallet ({ptr.PID}) was requested in iLog but {request.Pid} was picked.");
            }

            ptr.Complete(dateTime.Now);
            await repository.PalletTransferRequests.Update(ptr);

            var pallet = repository.StorageDetails.GetPalletDetail(ptr.PID) ?? throw new UnknownPIDException();
            pallet.Unrestrict();
            await repository.StorageDetails.Update(pallet);

            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();

            return await Task.FromResult(Unit.Value);
        }
        catch
        {
            repository.RollbackTransaction();
            throw;
        }
    }
}
