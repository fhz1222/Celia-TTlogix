using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using MediatR;

namespace Application.UseCases.Picking.PickPallet;

public class PickPalletCommand : IRequest
{
    public string ILogPalletPicked { get; set; }
    public string PickingJobNo { get; set; }
    public string Pid { get; set; }
}

public class PickPalletCommandHandler : IRequestHandler<PickPalletCommand>
{
    private readonly IPalletPicker palletPicker;
    private readonly IILogPickingRequestRepository repo;

    public PickPalletCommandHandler(IPalletPicker palletPicker, IILogPickingRequestRepository repo)
    {
        this.palletPicker = palletPicker;
        this.repo = repo;
    }

    public async Task<Unit> Handle(PickPalletCommand request, CancellationToken cancellationToken)
    {
        var pr = repo.GetLastRevision(request.PickingJobNo)
            ?? throw new ApplicationError($"Picking request {request.PickingJobNo} does not exist.");

        if (pr.IsClosed)
        {
            throw new ApplicationError($"Cannot pick pallet to a closed picking request {pr.PickingRequestId} rev {pr.Revision}.");
        }

        var parentPalletId = request.Pid != request.ILogPalletPicked ? request.ILogPalletPicked : null;
        await palletPicker.Pick(request.Pid, pr.OutboundJobNo, parentPalletId);

        return Unit.Value;
    }
}
