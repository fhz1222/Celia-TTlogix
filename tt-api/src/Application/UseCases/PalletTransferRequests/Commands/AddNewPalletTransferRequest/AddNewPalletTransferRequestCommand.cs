using Application.Exceptions;
using Application.Interfaces.Gateways;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using System;

namespace Application.UseCases.PalletTransferRequests.Commands.AddNewPalletTransferRequestCommand;

public class AddNewPalletTransferRequestCommand : IRequest<string>
{
    public string UserCode { get; set; } = null!;
    public string Pid { get; set; } = null!;
}

public class AddNewPalletTransferRequestCommandHandler : IRequestHandler<AddNewPalletTransferRequestCommand, string>
{
    private readonly IJobNumberGenerator jobNumberGenerator;
    private readonly IRepository repository;
    private readonly IDateTime dateTime;
    private readonly IILogConnectGateway iLogConnectGateway;


    public AddNewPalletTransferRequestCommandHandler(IJobNumberGenerator jobNumberGenerator, IRepository repository, IDateTime dateTime, IILogConnectGateway logConnectGateway)
    {
        this.jobNumberGenerator = jobNumberGenerator;
        this.repository = repository;
        this.dateTime = dateTime;
        this.iLogConnectGateway = logConnectGateway;
    }

    public async Task<string> Handle(AddNewPalletTransferRequestCommand request, CancellationToken cancellationToken)
    {
        if (!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new UnknownUserCodeException();
        try
        {
            repository.BeginTransaction();
            var jobNumber = jobNumberGenerator.GetJobNumber(repository.PalletTransferRequests);
            var pallet = repository.StorageDetails.GetPalletDetail(request.Pid)
                ?? throw new UnknownPIDException();
            if(!repository.Locations.IsILogStorageLocation(pallet.Location, pallet.WhsCode))
                throw new IllegalPalletLocationException("Pallet location must be on iLog Storage Location.");
            var ptrs = await repository.PalletTransferRequests.GetOngoing();
            if (ptrs.Any(ptr => ptr.PID == pallet.Id))
                throw new PalletCannotBeRequestedException("Pallet already requested.");
            pallet.TryToRequest();

            await repository.StorageDetails.Update(pallet);
            repository.PalletTransferRequests.Add(
                new(jobNumber, request.Pid, dateTime.Now, request.UserCode));
            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();
            iLogConnectGateway.PalletTransferRequestCreated(jobNumber);
            return jobNumber;
        }
        catch
        {
            repository.RollbackTransaction();
            throw;
        }
    }
}
