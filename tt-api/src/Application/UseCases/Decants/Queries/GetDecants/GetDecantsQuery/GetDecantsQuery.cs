using Application.Common.Models;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Decants.Queries.GetDecants;

public class GetDecantsQuery : IRequest<PaginatedList<DecantDto>>
{
    public PaginationQuery Pagination { get; set; } = null!;
    public string? OrderByExpression { get; set; }
    public bool OrderByDescending { get; set; }
    public DecantDtoFilter Filter { get; set; } = null!;
}

public class GetDecantsQueryHandler : IRequestHandler<GetDecantsQuery, PaginatedList<DecantDto>>
{
    private readonly IDecantRepository repository;

    public GetDecantsQueryHandler(IDecantRepository repository)
    {
        this.repository = repository;
    }

    public async Task<PaginatedList<DecantDto>> Handle(GetDecantsQuery request, CancellationToken cancellationToken)
    {
        // if sort order is not defined then sort results by JobNo (order should be the same as CreatedDate), first display the latest
        var orderBy = request.OrderByExpression ?? "JobNo";
        var descending = request.OrderByExpression == null ? true : request.OrderByDescending;
        var result = repository.GetDecants(request.Filter, request.Pagination, orderBy, descending);

        return await Task.FromResult(result);
    }
}

