using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.InboundReversalItems.Commands.AddNewInboundReversalItemCommand;

public class AddNewInboundReversalItemCommand : IRequest<Unit>
{
    public string[] PIDs { get; set; } = null!;
    public string JobNo { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class AddNewInboundReversalItemCommandHandler : IRequestHandler<AddNewInboundReversalItemCommand, Unit>
{
    private readonly IRepository repository;
    private readonly IDateTime dateTimeService;

    public AddNewInboundReversalItemCommandHandler(IDateTime dateTimeService, IRepository repository)
    {
        this.dateTimeService = dateTimeService;
        this.repository = repository;
    }

    public async Task<Unit> Handle(AddNewInboundReversalItemCommand request, CancellationToken cancellationToken)
    {
        if(!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new ApplicationError($"User {request.UserCode} does not exist.");

        repository.BeginTransaction();
        try
        {
            var inboundReversal = await repository.InboundReversals.GetInboundReversal(request.JobNo)
                ?? throw new ApplicationError($"Inbound reversal {request.JobNo} does not exist.");

            if(inboundReversal.Status == InboundReversalStatus.Completed)
                throw new ApplicationError($"Cannot modify completed reversal job.");

            if(inboundReversal.Status == InboundReversalStatus.Cancelled)
                throw new ApplicationError($"Cannot modify cancelled reversal job.");

            foreach(var PID in request.PIDs)
            {

                if(repository.InboundReversals.InboundReversalDetailExists(request.JobNo, PID))
                    throw new ApplicationError($"PID {PID} already exists in reversal {request.JobNo}");

                if(repository.InboundReversals.OutstandingInboundReversalExistsForPID(PID))
                    throw new ApplicationError($"PID {PID} is pending for completion in other reversal job.");

                var info = await repository.InboundReversals.GetPIDInfo(PID)
                    ?? throw new ApplicationError($"Unknown PID {PID}.");

                if(info.InJobNo != inboundReversal.InJobNo)
                    throw new ApplicationError($"PID {PID} is not from this reversal's inbound {inboundReversal.InJobNo}.");

                var newInboundReversalDetail = new InboundReversalDetail
                {
                    JobNo = request.JobNo,
                    CreatedBy = request.UserCode,
                    CreatedDate = dateTimeService.Now,
                    OriginalQty = info.OriginalQty,
                    Pid = info.Pid,
                    ProductCode = info.ProductCode
                };

                await repository.InboundReversals.AddNewInboundReversalDetail(newInboundReversalDetail);

                inboundReversal.Status = InboundReversalStatus.Processing;

                await repository.InboundReversals.UpdateInboundReversal(inboundReversal);
            }
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
