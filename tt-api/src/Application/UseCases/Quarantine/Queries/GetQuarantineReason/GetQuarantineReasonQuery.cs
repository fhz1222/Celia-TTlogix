using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Quarantine.Queries.GetQuarantineReason;

public class GetQuarantineReasonQuery : IRequest<string?>
{
    public string PID { get; set; } = null!;
}

public class GetQuarantineReasonQueryHandler : IRequestHandler<GetQuarantineReasonQuery, string?>
{
    private readonly IQuarantineRepository repository;

    public GetQuarantineReasonQueryHandler(IQuarantineRepository repository)
    {
        this.repository = repository;
    }

    public async Task<string?> Handle(GetQuarantineReasonQuery request, CancellationToken cancellationToken)
        => await Task.FromResult(repository.GetQuarantineReason(request.PID));
}
