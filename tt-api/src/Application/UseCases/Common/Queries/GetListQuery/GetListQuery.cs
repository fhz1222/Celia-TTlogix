using Application.Interfaces.Repositories;
using MediatR;
using Application.Common.Models;

namespace Application.UseCases.Common.Queries.GetListQuery;

public class GetListQuery<FILTER, T> : IRequest<PaginatedList<T>>
    where T : class
    where FILTER : class
{
    public FILTER? Filter { get; set; }
    public OrderBy? Sorting { get; set; }
    public PaginationQuery? Pagination { get; set; }
    public EntityType EntityType { get; set; }
}

public class GetListQueryHandler<FILTER, T> : IRequestHandler<GetListQuery<FILTER, T>, PaginatedList<T>>
    where T : class
    where FILTER : class
{
    private readonly IRepository repository;

    public GetListQueryHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public Task<PaginatedList<T>> Handle(GetListQuery<FILTER, T> request, CancellationToken cancellationToken)
    {                

        var list = repository.Metadata.GetPaginatedList<FILTER, T>(
            request.EntityType,
            request.Filter,
            request.Sorting,
            request.Pagination
        );

        return Task.FromResult(list);
    }
}
