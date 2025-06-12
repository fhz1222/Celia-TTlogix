using Application.Exceptions;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.ILogIntegration.Commands.GenerateBoxes;

public class GenerateBoxesCommand : IRequest<List<BoxDto>>
{
    public string Pid { get; set; }
}

public class GenerateBoxesCommandHandler : IRequestHandler<GenerateBoxesCommand, List<BoxDto>>
{
    private readonly IRepository repository;

    public GenerateBoxesCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<List<BoxDto>> Handle(GenerateBoxesCommand request, CancellationToken cancellationToken)
    {
        var pallet = repository.StorageDetails.GetPalletDetail(request.Pid) ?? throw new UnknownPIDException();
        if (!pallet.Product.IsCPart)
        {
            throw new ApplicationError($"Product {pallet.Product.Code} is not a C-Part material.");
        }
        var isInILog = repository.Locations.IsILogStorageLocation(pallet.Location, pallet.WhsCode);
        if (isInILog)
        {
            throw new ApplicationError($"Pallet {pallet.Id} is on iLog Storage Location.");
        }

        repository.BeginTransaction();
        try
        {
            repository.ILogBoxes.DeletePalletBoxes(pallet.Id);

            var numberOfBoxes = Math.Ceiling((decimal)pallet.Qty / pallet.Product.CPartBoxQty);
            var boxQuantities = GenerateBoxQuantities(pallet.Qty, pallet.Product.CPartBoxQty).ToList();

            if (numberOfBoxes != boxQuantities.Count)
            {
                throw new ApplicationError("Could not generate box identifiers");
            }

            boxQuantities.ForEach(qty => repository.ILogBoxes.CreateBox(pallet.Id, qty));

            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();
        }
        catch
        {
            repository.RollbackTransaction();
            throw;
        }

        return repository.ILogBoxes.GetBoxes(pallet.Id);
    }

    private IEnumerable<int> GenerateBoxQuantities(int total, int boxQty)
    {
        var remainder = total % boxQty;
        if (remainder == 0)
        {
            return Enumerable.Repeat(boxQty, total / boxQty);
        }
        else
        {
            return Enumerable.Repeat(boxQty, total / boxQty).Concat(Enumerable.Repeat(remainder, 1));
        }
    }
}
