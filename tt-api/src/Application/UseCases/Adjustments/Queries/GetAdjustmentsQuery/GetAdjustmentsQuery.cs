using Application.Common.Models;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Adjustments.Queries.GetAdjustmentsQuery;

public class GetAdjustmentsQuery : IRequest<PaginatedList<Adjustment>>
{
    public PaginationQuery Pagination { get; set; } = null!;
    public string? OrderByExpression { get; set; }
    public bool OrderByDescending { get; set; }
    public AdjustmentFilter Filter { get; set; } = null!;
}

public class GetCustomerAdjustmentsQueryHandler : IRequestHandler<GetAdjustmentsQuery, PaginatedList<Adjustment>>
{
    private readonly IAdjustmentRepository repository;

    public GetCustomerAdjustmentsQueryHandler(IAdjustmentRepository repository)
    {
        this.repository = repository;
    }

    public async Task<PaginatedList<Adjustment>> Handle(GetAdjustmentsQuery request, CancellationToken cancellationToken)
    {
        // if sort order is not defined then sort results by JobNo (order should be the same as CreatedDate), first display the latest
        var orderBy = request.OrderByExpression ?? "JobNo";
        var descending = request.OrderByExpression == null ? true : request.OrderByDescending;
        PaginatedList<Adjustment> result = repository.GetAdjustments(request.Filter, request.Pagination, orderBy, descending);

        return await Task.FromResult(result);
    }
}
