using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Metadata;
using Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Application.UseCases.Registration.Commands.ToggleActiveLocation;

public class ToggleActiveLocationCommand : IRequest<LocationDto>
{
    public string Code { get; set; } = null!;
    public string WarehouseCode { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class ToggleActiveLocationCommandHandler : IRequestHandler<ToggleActiveLocationCommand, LocationDto>
{
    private readonly IRepository repository;
    private readonly IDateTime dateTimeService;

    public ToggleActiveLocationCommandHandler(IRepository repository, IDateTime dateTimeService)
    {
        this.repository = repository;
        this.dateTimeService = dateTimeService;
    }

    public async Task<LocationDto> Handle(ToggleActiveLocationCommand request, CancellationToken cancellationToken)
    {
        if(!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new UnknownUserCodeException();

        repository.BeginTransaction();
        try
        {
            var metadata = repository.Metadata.Get<Metadata>(EntityType.Location, request.Code, request.WarehouseCode)
                ?? throw new ApplicationError($"Cannot find object for {request.Code}, {request.WarehouseCode}.");


            //Toggle Status
            if(metadata.Status == Status.Active)
            {
                var palletCount = repository.StorageDetails
                    .GetPalletCountOnLocationForRegistrationToggleActiveLocation(request.Code, request.WarehouseCode);

                if(palletCount > 0)
                {
                    throw new Exception("Failed to modify the location to Inactive as it contains PID in the location.");
                }
                else
                {
                    metadata.Status = Status.Inactive;
                    metadata.CancelledBy = request.UserCode;
                    metadata.CancelledDate = dateTimeService.Now;
                }
            }
            else
            {
                metadata.Status = Status.Active;
                metadata.CancelledBy = "";
                metadata.CancelledDate = null;
            }

            repository.Metadata.Update(EntityType.Location, metadata, new string[] { request.Code, request.WarehouseCode });

            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();

            return repository.Metadata.Get<LocationDto>(EntityType.Location, request.Code, request.WarehouseCode)
                ?? throw new ApplicationError($"Cannot find object for {request.Code}, {request.WarehouseCode}."); ;
        }
        catch(Exception)
        {
            repository.RollbackTransaction();
            throw;
        }
    }
}
