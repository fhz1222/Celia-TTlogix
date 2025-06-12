using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Application.UseCases.Common;
using Application.UseCases.Common.Queries.GetListQuery;
using Application.UseCases.StockTake.Queries.GetStockTakeMissingPid;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;
using System.Linq;

namespace Application.UseCases.StockTake.Commands.SendStandByNegative;

public class SendStandByNegativeCommand : IRequest<Unit>
{
    public string JobNo { get; set; } = null!;
    public string LocationCode { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class SendStandByNegativeCommandHandler : IRequestHandler<SendStandByNegativeCommand, Unit>
{
    private readonly IRepository repository;
    private readonly IDateTime dateTime;
    private readonly IMediator mediator;

    public SendStandByNegativeCommandHandler(IRepository repository, IDateTime dateTime, IMediator mediator)
    {
        this.repository = repository;
        this.dateTime = dateTime;
        this.mediator = mediator;
    }

    public async Task<Unit> Handle(SendStandByNegativeCommand request, CancellationToken cancellationToken)
    {
        if(!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new ApplicationError($"User {request.UserCode} does not exist.");

        repository.BeginTransaction();
        try
        {
            var stockTake = repository.Metadata.Get<Domain.Entities.StockTake>(EntityType.StockTake, request.JobNo)
                ?? throw new ApplicationError($"Unknown stock take JobNo {request.JobNo}.");

            if(stockTake.FixMissingBy.Trim() != "")
                throw new ApplicationError($"Job {request.JobNo} have been already sent to stand-by negative location.");

            var negativePids = await mediator.Send(new GetListQuery<GetStockTakeMissingPidFilter, StockTakeItemDto>()
            {
                Filter = new GetStockTakeMissingPidFilter { JobNo = request.JobNo },
                EntityType = EntityType.StockTakeItem,
                Sorting = null,
                Pagination = null,
            },
            cancellationToken)
                ?? throw new ApplicationError($"Error fetching missing pids {request.JobNo}."); ;

            var palletPids = negativePids.Items.Select(x => x.Pid).ToArray();

            var pallets = repository.StorageDetails.GetPallets(palletPids);

            foreach( var pallet in pallets )
            {
                var relocationLog = new StockTakeRelocationLog
                {
                    JobNo = request.JobNo,
                    Pid = pallet.Id,
                    TransType = StockTakeRelocationLogType.FixMissing,
                    OldLocationCode = pallet.Location,
                    OldWhscode = pallet.WhsCode,
                    NewLocationCode = request.LocationCode,
                    NewWhscode = request.WhsCode,
                    RelocatedBy = request.UserCode,
                    RelocatedDate = dateTime.Now
                };

                repository.Metadata.AddNew(EntityType.StockTakeRelocationLog, relocationLog);

                pallet.Location = request.LocationCode;
                pallet.WhsCode = request.WhsCode;
                await repository.StorageDetails.Update(pallet);
            }

            stockTake.FixMissingBy = request.UserCode;
            stockTake.FixMissingDate = dateTime.Now;
            repository.Metadata.Update(EntityType.StockTake, stockTake, request.JobNo);

            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();

            return Unit.Value;
        }
        catch(Exception)
        {
            repository.RollbackTransaction();
            throw;
        }
    }
}
