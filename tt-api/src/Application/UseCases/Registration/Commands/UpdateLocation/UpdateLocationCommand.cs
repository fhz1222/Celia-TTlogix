using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Metadata;
using Domain.ValueObjects;
using FluentValidation;
using MediatR;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace Application.UseCases.Registration.Commands.UpdateLocation;

public class UpdateLocationCommand : IRequest<LocationDto>
{
    public UpdateLocationDto Updated { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class UpdateLocationCommandHandler : IRequestHandler<UpdateLocationCommand, LocationDto>
{
    private readonly IRepository repository;
    private readonly IDateTime dateTimeService;
    private readonly IValidator<UpdateLocationDto>? validator;

    public UpdateLocationCommandHandler(IRepository repository, IDateTime dateTimeService, IValidator<UpdateLocationDto>? validator = null)
    {
        this.repository = repository;
        this.dateTimeService = dateTimeService;
        this.validator = validator;
    }

    public async Task<LocationDto> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
    {
        if(!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new UnknownUserCodeException();

        if(validator is not null && validator.Validate(request.Updated) is var validationResult && !validationResult.IsValid)
        {
            var errorMessage = string.Join(' ', validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ApplicationError(errorMessage);
        }

        repository.BeginTransaction();
        try
        {
            var location = repository.Metadata.Get<LocationDto>(EntityType.Location, request.Updated.Code, request.Updated.WarehouseCode)
                ?? throw new ApplicationError($"Cannot find object for {request.Updated.Code}, {request.Updated.WarehouseCode}.");

            var palletStatuses = repository.StorageDetails
                .GetPalletStatusesOnLocationForRegistrationUpdateLocation(location.Code, location.WarehouseCode);

            bool blDataChk = false;
            bool blStatusChk = false;

            if (location.Type == LocationType.Quarantine)
            {
                if (palletStatuses.Count != 0)
                {
                    //Check whether the status of the PID tally with the selection
                    if(request.Updated.Type == LocationType.Normal
                        || request.Updated.Type == LocationType.CrossDock
                        || request.Updated.Type == LocationType.Standby)
                    {
                        foreach (var status in palletStatuses)
                        {
                            if(status == StorageStatus.Quarantine)
                            {
                                throw new ApplicationError("Please empty the location and try again.");
                            }
                            else
                            {
                                blStatusChk = true;
                            }
                        }

                        //Check PID status not Quarantine then proceed to edit LocationType
                        if(blStatusChk)
                            blDataChk = true;
                    }
                    else
                    {
                        //Check PID status is Quarantine then proceed to edit LocationType
                        if(blStatusChk)
                            blDataChk = true;
                    }
                }
                else
                {
                    blDataChk = true;
                }
            }
            else
            {
                //If LocationType and PID's status not Quarantine, update registration
                if(request.Updated.Type == LocationType.Quarantine)
                {
                    if(palletStatuses.Count != 0)
                    {
                        foreach(var status in palletStatuses)
                        {
                            if (status != StorageStatus.Quarantine)
                            {
                                throw new ApplicationError("Please empty the location and try again.");
                            }
                            else
                                blStatusChk = true;
                        }
                        if(blStatusChk)
                            blDataChk = true;
                    }
                    else
                        blDataChk = true;
                }
                else
                {
                    blDataChk = true;
                }
            }

            //If no data is exist under the location, update registration
            if(blDataChk)
            {
                location.Code = request.Updated.Code;
                location.Name = request.Updated.Name;
                location.WarehouseCode = request.Updated.WarehouseCode;
                location.AreaCode = request.Updated.AreaCode;
                location.Status = request.Updated.Status;
                location.Type = request.Updated.Type;
                location.M3 = request.Updated.M3;
                location.IsPriority = request.Updated.IsPriority;

                repository.Metadata.Update(EntityType.Location, location, new string[] { location.Code, location.WarehouseCode });
            }

            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();
            return location;
        }
        catch(Exception)
        {
            repository.RollbackTransaction();
            throw;
        }
    }
}
