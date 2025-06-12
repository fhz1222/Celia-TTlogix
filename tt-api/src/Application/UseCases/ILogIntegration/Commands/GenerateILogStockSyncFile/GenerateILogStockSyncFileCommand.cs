using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Services;
using CsvHelper;
using CsvHelper.Configuration;
using MediatR;
using System.Globalization;

namespace Application.UseCases.ILogIntegration.Commands.GenerateILogStockSyncFile;

public class GenerateILogStockSyncFileCommand : IRequest<NamedStream>
{
}

public class GenerateILogStockSyncFileCommandHandler : IRequestHandler<GenerateILogStockSyncFileCommand, NamedStream>
{
    private readonly IRepository repository;

    public GenerateILogStockSyncFileCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<NamedStream> Handle(GenerateILogStockSyncFileCommand request, CancellationToken cancellationToken)
    {
        var isEnabled = repository.ILogIntegrationRepository.GetStatus();
        if (isEnabled)
        {
            throw new ApplicationError("Stock File cannot be generated when iLog integration is enabled.");
        }

        var iLogStorageCategoryId = repository.Locations.GetILogStorageLocationCategoryId();
        var warehouses = repository.ILogIntegrationRepository.GetWarehouses();
        var pallets = repository.StorageDetails.GetILogStockSynchronizationData(warehouses, iLogStorageCategoryId);

        // generate boxes
        var alreadyGenerated = repository.ILogBoxes.AreBoxesGenerated();
        if (!alreadyGenerated)
        {
            repository.RunInTransaction(() =>
            {
                var cpartPallets = pallets.Where(p => p.IsCpart).ToList();
                cpartPallets.ForEach(pallet =>
                {
                    var boxQuantities = ILogBoxQtyGenerator.Generate(pallet.Quantity, pallet.CPartBoxQty).ToList();
                    boxQuantities.ForEach(qty => repository.ILogBoxes.CreateBox(pallet.PID, qty));
                });
            });
        }

        // combine pallets with boxes
        var pids = pallets.Select(p => p.PID).ToArray();
        var boxes = repository.ILogBoxes.GetBoxes(pids);
        var items =
            from p in pallets
            join box in boxes on p.PID equals box.MasterPalletId into joinedBoxes
            from box in joinedBoxes.DefaultIfEmpty()
            select new ILogStockSyncItem(p, box);

        // generate stock file
        var stream = new MemoryStream();
        var outputFile = new StreamWriter(stream);
        var config = new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ";" };
        var csvWriter = new CsvWriter(outputFile, config);
        csvWriter.WriteRecords(items);
        outputFile.Flush();
        stream.Position = 0;

        var fileName = $"stock_{DateTime.UtcNow:yyyyMMddTHHmmss}.csv";
        return await Task.FromResult(new NamedStream
        {
            Name = fileName,
            Stream = stream
        });
    }
}
