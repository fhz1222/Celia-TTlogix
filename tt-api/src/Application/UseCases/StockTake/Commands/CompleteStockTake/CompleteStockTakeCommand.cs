using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Application.UseCases.Common;
using Domain.Metadata;
using MediatR;

namespace Application.UseCases.StockTake.Commands.CompleteStockTake;

public class CompleteStockTakeCommand : IRequest<Domain.Entities.StockTake>
{
    public string JobNo { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class CompleteStockTakeCommandHandler : IRequestHandler<CompleteStockTakeCommand, Domain.Entities.StockTake>
{
    private readonly IRepository repository;
    private readonly IDateTime dateTime;

    public CompleteStockTakeCommandHandler(IRepository repository, IDateTime dateTime)
    {
        this.repository = repository;
        this.dateTime = dateTime;
    }

    public async Task<Domain.Entities.StockTake> Handle(CompleteStockTakeCommand request, CancellationToken cancellationToken)
    {
        if(!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new ApplicationError($"User {request.UserCode} does not exist.");

        repository.BeginTransaction();
        try
        {
            var jobMetadata = repository.Metadata.Get<JobMetadata>(EntityType.StockTake, request.JobNo)
                ?? throw new ApplicationError($"Unknown stock take JobNo {request.JobNo}.");

            jobMetadata.Complete(request.UserCode, dateTime.Now);

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
