using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Application.UseCases.Common;
using Domain.Metadata;
using MediatR;

namespace Application.UseCases.StockTake.Commands.CancelStockTake;

public class CancelStockTakeCommand : IRequest<Domain.Entities.StockTake>
{
    public string JobNo { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class CancelStockTakeCommandHandler : IRequestHandler<CancelStockTakeCommand, Domain.Entities.StockTake>
{
    private readonly IRepository repository;
    private readonly IDateTime dateTime;

    public CancelStockTakeCommandHandler(IRepository repository, IDateTime dateTime)
    {
        this.repository = repository;
        this.dateTime = dateTime;
    }

    public async Task<Domain.Entities.StockTake> Handle(CancelStockTakeCommand request, CancellationToken cancellationToken)
    {
        if(!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new ApplicationError($"User {request.UserCode} does not exist.");

        repository.BeginTransaction();
        try
        {
            var jobMetadata = repository.Metadata.Get<JobMetadata>(EntityType.StockTake, request.JobNo)
                ?? throw new ApplicationError($"Unknown stock take JobNo {request.JobNo}.");

            if (jobMetadata.Status == Domain.ValueObjects.JobStatus.Completed
                || jobMetadata.Status == Domain.ValueObjects.JobStatus.Cancelled)
                throw new ApplicationError($"Cannot cancel completed or already cancelled stock take {request.JobNo}.");

            jobMetadata.Cancel(request.UserCode, dateTime.Now);

            repository.Metadata.Update(EntityType.StockTake, jobMetadata, request.JobNo);

            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();

            var stockTake = repository.Metadata.Get<Domain.Entities.StockTake>(EntityType.StockTake, request.JobNo)
                ?? throw new ApplicationError($"Unknown stock take JobNo {request.JobNo}.");
            return stockTake;
        }
        catch(Exception)
        {
            repository.RollbackTransaction();
            throw;
        }
    }
}
