using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.InvoiceRequest.Commands.Unblock;

public class UnblockCommand : IRequest
{
    public string JobNo { get; set; } = default!;
}

public class UnblockCommandHandler : IRequestHandler<UnblockCommand>
{
    private readonly IInvoiceRequestRepository repository;

    public UnblockCommandHandler(IInvoiceRequestRepository repository)
    {
        this.repository = repository;
    }

    public Task<Unit> Handle(UnblockCommand request, CancellationToken cancellationToken)
    {
        repository.Unblock(request.JobNo);
        return Task.FromResult(Unit.Value);
    }
}
