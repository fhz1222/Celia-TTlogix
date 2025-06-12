using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Adjustments.Queries.GetAdjustmentsQuery;

public class GetAdjustmentDetailsQuery : IRequest<Adjustment>
{
    public string JobNo { get; set; } = null!;
}

public class GetAdjustmentDetailsQueryHandler : IRequestHandler<GetAdjustmentDetailsQuery, Adjustment?>
{
    private readonly IAdjustmentRepository repository;

    public GetAdjustmentDetailsQueryHandler(IAdjustmentRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Adjustment?> Handle(GetAdjustmentDetailsQuery request, CancellationToken cancellationToken)
        => await Task.FromResult(repository.GetAdjustmentDetails(request.JobNo));
}
