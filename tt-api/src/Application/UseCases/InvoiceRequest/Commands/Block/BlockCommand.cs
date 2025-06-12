using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.InvoiceRequest.Commands.Block;

public class BlockCommand : IRequest
{
    public string JobNo { get; set; } = default!;
    public string UserCode { get; set; } = default!;
}

public class BlockCommandHandler : IRequestHandler<BlockCommand>
{
    private readonly IInvoiceRequestRepository repository;

    public BlockCommandHandler(IInvoiceRequestRepository repository) => this.repository = repository;

    public async Task<Unit> Handle(BlockCommand request, CancellationToken cancellationToken)
    {
        await repository.Block(request.JobNo, request.UserCode);
        return await Task.FromResult(Unit.Value);
    }
}
