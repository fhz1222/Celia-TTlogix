using Application.Exceptions;
using Application.Interfaces.Repositories;
using Domain.Exceptions;
using MediatR;

namespace Application.UseCases.AdjustmentItems.Commands.PrepareNewAdjustmentItemCommand;

public class PrepareNewAdjustmentItemCommand : IRequest<AdjustmentItemWithPalletDto>
{
    public string JobNo { get; set; } = null!;
    public string PID { get; set; } = null!;
}

public class PrepareNewAdjustmentItemCommandHandler : IRequestHandler<PrepareNewAdjustmentItemCommand, AdjustmentItemWithPalletDto>
{
    private readonly IRepository repository;

    public PrepareNewAdjustmentItemCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<AdjustmentItemWithPalletDto> Handle(PrepareNewAdjustmentItemCommand request, CancellationToken cancellationToken)
    {
        var adjustment = this.repository.Adjustments.GetAdjustmentDetails(request.JobNo) ?? throw new UnknownJobNoException();
        if (!adjustment.CanAddItem)
            throw new IllegalAdjustmentChangeException();
        var pallet = this.repository.StorageDetails.GetPalletDetail(request.PID) ?? throw new UnknownPIDException();
        if (!pallet.CanBeAdjusted(adjustment.WhsCode, adjustment.CustomerCode, adjustment.JobType))
            throw new IncorrectPalletException();
        if (repository.Locations.IsILogInboundLocation(pallet.Location, pallet.WhsCode))
            throw new IllegalAdjustmentChangeException($"Cannot adjust PID on ILog Inbound location.");
        return new AdjustmentItemWithPalletDto
        {
            JobNo = adjustment.JobNo,
            Pallet = pallet,
            NewQty = pallet.Qty,
            NewQtyPerPkg = pallet.QtyPerPkg,
        };
    }
}
