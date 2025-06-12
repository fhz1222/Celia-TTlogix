using Application.Common.Models;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.RelocationLogs.Queries.GetRelocationLogs;

public class GetRelocationLogsQuery : IRequest<PaginatedList<RelocationLogDto>>
{
    public PaginationQuery Pagination { get; set; } = null!;
    public RelocationLogDtoFilter Filter { get; set; }
    public string? OrderByExpression { get; set; }
    public bool OrderByDescending { get; set; }
}

public class GetRelocationLogsQueryHandler : IRequestHandler<GetRelocationLogsQuery, PaginatedList<RelocationLogDto>>
{
    private readonly IRelocationRepository repository;

    public GetRelocationLogsQueryHandler(IRelocationRepository repository)
    {
        this.repository = repository;
    }

    public async Task<PaginatedList<RelocationLogDto>> Handle(GetRelocationLogsQuery request, CancellationToken cancellationToken)
    {
        if (!request.Filter.MandatoryConditionsAreSet)
            throw new MandatoryFilterNotSetException();
        if (!request.Filter.DatesAreValid)
        {
            throw new IllegalDateFilterException();
        }
        PaginatedList<RelocationLogDto> result = repository.GetRelocationLogs(request.Pagination,
                    request.Filter, request.OrderByExpression, request.OrderByDescending);
        return await Task.FromResult(result);
    }
}
