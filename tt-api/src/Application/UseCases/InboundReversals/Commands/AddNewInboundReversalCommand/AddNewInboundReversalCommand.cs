using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.InboundReversals.Commands.AddNewInboundReversalCommand;

public class AddNewInboundReversalCommand : IRequest<InboundReversal>
{
    public string InJobNo { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class AddNewInboundReversalCommandHandler : IRequestHandler<AddNewInboundReversalCommand, InboundReversal>
{
    private readonly IJobNumberGenerator jobNumberGenerator;
    private readonly IRepository repository;

    public AddNewInboundReversalCommandHandler(IJobNumberGenerator jobNumberGenerator, IRepository repository)
    {
        this.jobNumberGenerator = jobNumberGenerator;
        this.repository = repository;
    }

    public async Task<InboundReversal> Handle(AddNewInboundReversalCommand request, CancellationToken cancellationToken)
    {
        repository.BeginTransaction();
        try
        {
            var inbound = await repository.InboundReversals.GetInboundInfo(request.InJobNo)
            ?? throw new ApplicationError($"Unknown inbound JobNo {request.InJobNo}.");

            if(inbound.Status != InboundStatus.Completed)
                throw new ApplicationError($"Inbound must be completed.");

            var jobNumber = jobNumberGenerator.GetJobNumber(repository.InboundReversals);
            var newInboundReversal = new InboundReversal
            {
                JobNo = jobNumber,
                InJobNo = request.InJobNo,
                CreatedBy = request.UserCode,
                RefNo = inbound.RefNo.Length > 20 ? inbound.RefNo[..20] : inbound.RefNo,
                CustomerCode = inbound.CustomerCode,
                SupplierId = inbound.SupplierId,
                WhsCode = inbound.WhsCode,
                CreatedDate = DateTime.Now,
                Status = InboundReversalStatus.New,
                Reason = "",
                ConfirmedBy = "",
                ConfirmedDate = null,
                CancelledBy = "",
                CancelledDate = null,
            };

            await repository.InboundReversals.AddNewInboundReversal(newInboundReversal);
            repository.CommitTransaction();

            return newInboundReversal;
        }
        catch(Exception)
        {
            repository.RollbackTransaction();
            throw;
        }
    }
}
