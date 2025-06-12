using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Application.Services;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.InboundReversals.Commands.CancelInboundReversalCommand;

public class CancelInboundReversalCommand : IRequest<InboundReversal>
{
    public string JobNo { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class AddNewInboundReversalCommandHandler : IRequestHandler<CancelInboundReversalCommand, InboundReversal>
{
    private readonly IRepository repository;
    private IDateTime dateTimeService;

    public AddNewInboundReversalCommandHandler(IRepository repository, IDateTime dateTimeService)
    {
        this.repository = repository;
        this.dateTimeService = dateTimeService;
    }

    public async Task<InboundReversal> Handle(CancelInboundReversalCommand request, CancellationToken cancellationToken)
    {
        if(!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new ApplicationError($"User {request.UserCode} does not exist.");

        repository.BeginTransaction();
        try
        {
            var inboundReversal = await repository.InboundReversals.GetInboundReversal(request.JobNo)
                ?? throw new ApplicationError($"Unknown inbound reversal JobNo {request.JobNo}.");

            if(inboundReversal.Status == InboundReversalStatus.Completed)
                throw new ApplicationError($"Cannot modify completed inbound reversal.");

            if(inboundReversal.Status == InboundReversalStatus.Cancelled)
                throw new ApplicationError($"Cannot modify cancelled inbound reversal.");

            var anyDetailsExists = await repository.InboundReversals
                .AnyInboundReversalDetailsExists(inboundReversal.JobNo);

            if(anyDetailsExists)
                throw new ApplicationError($"Cannot cancel inbound reversal with details. Delete details and try again.");

            inboundReversal.Cancel(request.UserCode, dateTimeService.Now);
            await repository.InboundReversals.UpdateInboundReversal(inboundReversal);

            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();
        }
        catch(Exception)
        {
            repository.RollbackTransaction();
            throw;
        }

        var updatedReversal = await repository.InboundReversals.GetInboundReversal(request.JobNo)
            ?? throw new ApplicationError($"Unknown inbound reversal JobNo {request.JobNo}.");
        return updatedReversal;
    }
}
