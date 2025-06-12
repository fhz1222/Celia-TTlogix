using Application.Exceptions;
using Application.Interfaces.Repositories;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.InboundReversalItems.Commands.DeleteInboundReversalItemCommand;

public class DeleteInboundReversalItemCommand : IRequest
{
    public string JobNo { get; set; } = null!;
    public string PID { get; set; } = null!;
}

public class DeleteInboundReversalItemCommandHandler : IRequestHandler<DeleteInboundReversalItemCommand, Unit>
{
    private readonly IRepository repository;

    public DeleteInboundReversalItemCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Unit> Handle(DeleteInboundReversalItemCommand request, CancellationToken cancellationToken)
    {
        repository.BeginTransaction();
        try
        {
            var inboundReversal = await repository.InboundReversals.GetInboundReversal(request.JobNo)
                ?? throw new ApplicationError($"Inbound reversal {request.JobNo} does not exist.");

            if(inboundReversal.Status == InboundReversalStatus.Completed)
                throw new ApplicationError($"Cannot modify completed reversal job.");

            if(inboundReversal.Status == InboundReversalStatus.Cancelled)
                throw new ApplicationError($"Cannot modify cancelled reversal job.");

            await repository.InboundReversals.DeleteInboundReversalDetail(request.JobNo, request.PID);
            await repository.SaveChangesAsync();

            var itemsCount = repository.InboundReversals.GetInboundReversalDetailsCount(request.JobNo);
            if(itemsCount == 0)
            {
                inboundReversal.Status = InboundReversalStatus.New;
                await repository.InboundReversals.UpdateInboundReversal(inboundReversal);
            }
            else
            {
                inboundReversal.Status = InboundReversalStatus.Processing;
                await repository.InboundReversals.UpdateInboundReversal(inboundReversal);
            }
            await repository.SaveChangesAsync();
            repository.CommitTransaction();
        }
        catch(Exception)
        {
            repository.RollbackTransaction();
            throw;
        }
        return Unit.Value;
    }
}
