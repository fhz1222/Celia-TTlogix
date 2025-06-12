using Application.Exceptions;
using Application.Extensions;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Relocation.Commands;

public class ForceRelocatePalletFromiLogCommand : IRequest<Pallet>
{
    public string PID { get; set; } = null!;
    public string NewLocation { get; set; } = null!;
    public string RelocatedBy { get; set; } = null!;
    public DateTime RelocatedOn { get; set; }
    public string? AllowedTrgLocationCategory { get; set; }
}

public class ForceRelocatePalletFromiLogCommandHandler : IRequestHandler<ForceRelocatePalletFromiLogCommand, Pallet>
{
    private readonly IRepository repository;
    private readonly ILogger<ForceRelocatePalletFromiLogCommandHandler> logger;

    public ForceRelocatePalletFromiLogCommandHandler(ILogger<ForceRelocatePalletFromiLogCommandHandler> logger, IRepository repository)
    {
        this.repository = repository;
        this.logger = logger;
    }

    public async Task<Pallet> Handle(ForceRelocatePalletFromiLogCommand request, CancellationToken cancellationToken)
    {
        var pallet = repository.StorageDetails.GetPalletDetail(request.PID)
            ?? throw new UnknownPIDException();
        if (pallet.Status.NotIn(StorageStatus.Putaway, StorageStatus.Quarantine))
            logger.LogWarning("Relocating {status} pallet {pid}.", pallet.Status, pallet.Id);

        var trgLocation = repository.Locations.GetLocation(request.NewLocation, pallet.WhsCode)
            ?? throw new UnknownLocationException($"Target location ({pallet.WhsCode}/{request.NewLocation}) unknown.");
        if (trgLocation.IsInactive)
        {
            logger.LogWarning("Relocating pallet {pid} to inactive location {loc}.", pallet.Id, trgLocation.Code);
        }

        var srcLocation = repository.Locations.GetLocation(pallet.Location, pallet.WhsCode)
            ?? throw new UnknownLocationException($"Source location ({pallet.WhsCode}/{pallet.Location}) unknown.");

        var isAllowedSource =
            repository.Locations.IsILogStorageLocation(srcLocation.Code, srcLocation.WarehouseCode)
            || repository.Locations.IsILogInboundLocation(srcLocation.Code, srcLocation.WarehouseCode);
        if (!isAllowedSource)
        {
            var srcLocCatName = repository.Locations.GetILogLocationCategoryName(srcLocation.Code, srcLocation.WarehouseCode);
            throw new IllegalLocationException($"Source location ({pallet.WhsCode}/{pallet.Location}) category must be iLogStorage or Inbound but is {srcLocCatName}.");
        }

        if(request.AllowedTrgLocationCategory.IsNotEmpty())
            if(!repository.Locations.IsLocationOfCategory(trgLocation.Code, trgLocation.WarehouseCode, request.AllowedTrgLocationCategory))
            {
                var trgLocCatName = repository.Locations.GetILogLocationCategoryName(trgLocation.Code, trgLocation.WarehouseCode);
                throw new IllegalLocationException($"Target location ({trgLocation.WarehouseCode}/{trgLocation.Code}) category ({trgLocCatName}) not allowed.");
            }


        try
        {
            repository.BeginTransaction();

            pallet.RelocateTo(trgLocation);
            await repository.StorageDetails.Update(pallet);

            var relocationLog = new RelocationLog()
            {
                PalletId = pallet.Id,
                ExternalPalletId = string.Empty, //always assume to be internal
                SourceLocation = srcLocation,
                TargetLocation = trgLocation,
                //todo: maybe parametrize?
                ScannerType = ScannerType.ILogScanner,
                RelocatedBy = request.RelocatedBy,
                RelocatedOn = request.RelocatedOn
            };
            await repository.Relocations.AddRelocationLog(relocationLog);

            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();
        }
        catch
        {
            repository.RollbackTransaction();
        }


        return pallet;
    }
}