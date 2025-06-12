using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Domain.Exceptions;
using MediatR;

namespace Application.UseCases.Quarantine.Commands.UpdateQuarantineReason;

public class UpdateQuarantineReasonCommand : IRequest
{
    public string[] PIDS { get; set; } = null!;
    public string Reason { get; set; } = null!;
    public string UserCode { get; set; }
}

public class UpdateQuarantineReasonCommandHandler : IRequestHandler<UpdateQuarantineReasonCommand>
{
    private readonly IRepository repository;
    private readonly IDateTime datetTimeService;

    public UpdateQuarantineReasonCommandHandler(IRepository repository, IDateTime datetTimeService)
    {
        this.repository = repository;
        this.datetTimeService = datetTimeService;
    }

    public async Task<Unit> Handle(UpdateQuarantineReasonCommand request, CancellationToken cancellationToken)
    {
        foreach(var pid in request.PIDS) {
            var pallet = repository.StorageDetails.GetPalletDetail(pid);
        if (pallet == null)
            throw new UnknownPIDException();

        if (!pallet.IsInQuarantine)
            throw new IncorrectPalletException($"PID {pallet} is not in quarantine");

            await repository.Quarantine.SetQuarantineReason(pid, request.Reason, datetTimeService.Now);
        }
        await repository.SaveChangesAsync();
        return await Task.FromResult(Unit.Value);
    }
}
