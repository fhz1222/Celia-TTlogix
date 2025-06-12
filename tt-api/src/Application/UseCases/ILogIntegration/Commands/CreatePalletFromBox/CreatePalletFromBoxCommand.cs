using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.ILogIntegration.Commands.CreatePalletFromBox;

public class CreatePalletFromBoxCommand : IRequest<string>
{
    public string BoxId { get; set; }
}

public class CreatePalletFromBoxCommandHandler : IRequestHandler<CreatePalletFromBoxCommand, string>
{
    private readonly IRepository repository;
    private readonly IPIDGenerator pidGenerator;
    private readonly ILogger<CreatePalletFromBoxCommandHandler> logger;

    public CreatePalletFromBoxCommandHandler(IRepository repository, IPIDGenerator pidGenerator, ILogger<CreatePalletFromBoxCommandHandler> logger)
    {
        this.repository = repository;
        this.pidGenerator = pidGenerator;
        this.logger = logger;
    }

    public async Task<string> Handle(CreatePalletFromBoxCommand request, CancellationToken cancellationToken)
    {
        var box = repository.ILogBoxes.GetBox(request.BoxId) ?? throw new ApplicationError($"Box {request.BoxId} not found.");
        var pallet = repository.StorageDetails.GetPalletDetail(box.MasterPalletId)
            ?? throw new ApplicationError($"Pallet {box.MasterPalletId} not found.");

        if (box.Qty == pallet.Qty)
        {
            // no need to repack - returning box's original pallet
            repository.ILogBoxes.DeleteBoxes(new string[] { box.BoxId });
            return await Task.FromResult(box.MasterPalletId);
        }

        repository.BeginTransaction();
        try
        {
            // create new pallet
            var pid = pidGenerator.GetNewPIDs(repository, 1)[0];
            var newPallet = CreatePallet(pallet, pid, box.Qty);
            await repository.StorageDetails.AddNewPallet(newPallet, pallet.Id);
            repository.ILogBoxes.DeleteBoxes(new string[] { box.BoxId });

            // reduce parent pallet qty
            pallet.OriginalQty -= newPallet.OriginalQty;
            pallet.Qty -= newPallet.Qty;
            pallet.QtyPerPkg -= newPallet.QtyPerPkg;
            await repository.StorageDetails.Update(pallet);

            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();

            return await Task.FromResult(pid);
        }
        catch
        {
            repository.RollbackTransaction();
            throw;
        }
    }

    private Pallet CreatePallet(Pallet pallet, string newPid, int newQty)
    {
        return new Pallet()
        {
            Id = newPid,
            Qty = newQty,
            QtyPerPkg = newQty,
            OriginalQty = newQty,
            AllocatedQty = 0,
            Product = pallet.Product,
            Status = StorageStatus.Putaway,
            Length = pallet.Length,
            Width = pallet.Width,
            Height = pallet.Height,
            NetWeight = pallet.NetWeight,
            GrossWeight = pallet.GrossWeight,
            InboundJobNo = pallet.InboundJobNo,
            OutboundJobNo = string.Empty
        };
    }
}