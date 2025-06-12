using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Adjustments.Commands.CancelAdjustmentCommand;

public class CancelAdjustmentCommand : IRequest<Adjustment>
{
    public string JobNo { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class CancelAdjustmentCommandHandler : IRequestHandler<CancelAdjustmentCommand, Adjustment>
{
    private readonly IRepository repository;
    private readonly IDateTime dateTimeService;

    public CancelAdjustmentCommandHandler(IRepository repository, IDateTime dateTimeService)
    {
        this.repository = repository;
        this.dateTimeService = dateTimeService;
    }

    public async Task<Adjustment> Handle(CancelAdjustmentCommand request, CancellationToken cancellationToken)
    {
        // check if user code exists 
        if (!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new UnknownUserCodeException();

        repository.BeginTransaction();
        var adjustment = repository.Adjustments.GetAdjustmentDetails(request.JobNo);
        try
        {
            if (adjustment == null)
                throw new UnknownJobNoException();

            adjustment.Cancel(request.UserCode, dateTimeService.Now);
            await repository.Adjustments.Update(adjustment);
            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();
            return adjustment;
        }
        catch (Exception)
        {
            repository.RollbackTransaction();
            throw;
        }
    }
}
