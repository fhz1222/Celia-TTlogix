using Application.Exceptions;
using Application.Extensions;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.ILogIntegration.Queries.GetBoxes;

public class GetBoxesQuery : IRequest<List<BoxDto>>
{
    public string Pid { get; set; }
}

public class GetBoxesQueryHandler : IRequestHandler<GetBoxesQuery, List<BoxDto>>
{
    private readonly IRepository repository;

    public GetBoxesQueryHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public Task<List<BoxDto>> Handle(GetBoxesQuery request, CancellationToken cancellationToken)
    {
        var pallet = repository.StorageDetails.GetPalletDetail(request.Pid) ?? throw new UnknownPIDException();
        if (!pallet.Product.IsCPart)
        {
            throw new ApplicationError($"Product {pallet.Product.Code} is not a C-Part material.");
        }

        var boxes = repository.ILogBoxes.GetBoxes(request.Pid);
        if (boxes.None())
        {
            throw new ApplicationError($"No boxes found for pallet {pallet.Id}.");
        }

        var boxesQty = boxes.Sum(b => b.Qty);
        if (boxesQty != pallet.Qty)
        {
            throw new ApplicationError($"Boxes qty ({boxesQty}) is not equal to pallet qty ({pallet.Qty}).");
        }

        return Task.FromResult(boxes);
    }
}