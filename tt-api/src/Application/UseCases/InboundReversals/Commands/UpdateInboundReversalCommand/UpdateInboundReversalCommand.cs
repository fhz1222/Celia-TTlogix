using Application.Exceptions;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.InboundReversals.Commands.UpdateInboundReversalCommand;

public class UpdateInboundReversalCommand : IRequest<InboundReversal>
{
    public string JobNo { get; set; } = null!;
    public string RefNo { get; set; } = null!;
    public string? Reason { get; set; } = null!;
}

public class UpdateInboundReversalCommandHandler : IRequestHandler<UpdateInboundReversalCommand, InboundReversal>
{
    private readonly IRepository repository;

    public UpdateInboundReversalCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<InboundReversal> Handle(UpdateInboundReversalCommand request, CancellationToken cancellationToken)
    {
        repository.BeginTransaction();
        try
        {
            var inboundReversal = await repository.InboundReversals.GetInboundReversal(request.JobNo)
                ?? throw new ApplicationError($"Unknown inbound reversal JobNo {request.JobNo}.");

            if(inboundReversal.Status == InboundReversalStatus.Completed)
                throw new ApplicationError($"Cannot modify completed inbound reversal.");

            if(inboundReversal.Status == InboundReversalStatus.Cancelled)
                throw new ApplicationError($"Cannot modify cancelled inbound reversal.");

            inboundReversal.RefNo = request.RefNo;
            inboundReversal.Reason = request.Reason ?? "";

            await repository.InboundReversals.UpdateInboundReversal(inboundReversal);
            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();
            return inboundReversal;
        }
        catch(Exception)
        {
            repository.RollbackTransaction();
            throw;
        }
    }
}
