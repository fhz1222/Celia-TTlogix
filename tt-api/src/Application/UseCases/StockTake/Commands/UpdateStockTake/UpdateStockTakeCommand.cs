using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.UseCases.Common;
using MediatR;

namespace Application.UseCases.StockTake.Commands.UpdateStockTake;

public class UpdateStockTakeCommand : IRequest<Domain.Entities.StockTake>
{
    public UpdateStockTakeDto Updated { get; set; } = null!;
}

public class UpdateStockTakeCommandHandler : IRequestHandler<UpdateStockTakeCommand, Domain.Entities.StockTake>
{
    private readonly IRepository repository;

    public UpdateStockTakeCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Domain.Entities.StockTake> Handle(UpdateStockTakeCommand request, CancellationToken cancellationToken)
    {
        if(request.Updated.RefNo.Length > 30)
            throw new ApplicationError("Stock Take RefNo can be max 30 characters.");

        if(request.Updated.Remark.Length > 100)
            throw new ApplicationError("Stock Take Remark can be max 100 characters.");

        repository.BeginTransaction();
        try
        {
            repository.Metadata.Update(EntityType.StockTake, request.Updated, request.Updated.JobNo);

            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();

            var stockTake = repository.Metadata.Get<Domain.Entities.StockTake>(EntityType.StockTake, request.Updated.JobNo)
                ?? throw new ApplicationError($"Unknown stock take JobNo {request.Updated.JobNo}.");
            return stockTake;
        }
        catch(Exception)
        {
            repository.RollbackTransaction();
            throw;
        }
    }
}
