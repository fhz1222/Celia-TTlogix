using Application.Common.Models;
using Application.Exceptions;
using Application.Extensions;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Storage.Queries.GetStorageDetailItems;

public class GetStorageDetailItemsQuery : IRequest<StorageDetailPaginatedList>
{
    public PaginationQuery Pagination { get; set; } = null!;
    public StorageDetailItemDtoFilter Filter { get; set; } = null!;
    public string? OrderByExpression { get; set; }
    public bool OrderByDescending { get; set; }
}

public class GetStorageDetailItemsQueryHandler : IRequestHandler<GetStorageDetailItemsQuery, StorageDetailPaginatedList>
{
    private readonly IStorageDetailRepository storageDetailRepository;

    public GetStorageDetailItemsQueryHandler(IStorageDetailRepository storageDetailRepository)
    {
        this.storageDetailRepository = storageDetailRepository;
    }

    public async Task<StorageDetailPaginatedList> Handle(GetStorageDetailItemsQuery request, CancellationToken cancellationToken)
    {
        var filter = request.Filter;
        if (filter.ProductCode.IsEmpty() && filter.Location.IsEmpty() && filter.PID.IsEmpty() && filter.ExternalPID.IsEmpty())
            throw new RequiredFilterException("Invalid search criteria - Please enter at least the PID, ExternalPID, Part No or the Location for this view");

        var result = storageDetailRepository.GetStorageDetailItems(request.Pagination, request.Filter, request.OrderByExpression, request.OrderByDescending);
        return await Task.FromResult(result);
    }
}
