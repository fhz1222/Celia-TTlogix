using Application.Common.Models;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.UseCases.Storage.Queries.GetStorageDetailItems;
using MediatR;

namespace Application.UseCases.StorageDetails.Queries.GetStorageDetailItems;

public class GetStorageDetailItemsWithPartInfoQuery : IRequest<PaginatedList<StorageDetailItemWithPartInfoDto>>
{
    public PaginationQuery Pagination { get; set; } = null!;
    public StorageDetailItemWithPartInfoDtoFilter Filter { get; set; } = null!;
    public string? OrderByExpression { get; set; }
    public bool OrderByDescending { get; set; }
}

public class GetStorageDetailItemsWithPartInfoQueryHandler : IRequestHandler<GetStorageDetailItemsWithPartInfoQuery, PaginatedList<StorageDetailItemWithPartInfoDto>>
{
    private readonly IStorageDetailRepository storageDetailRepository;

    public GetStorageDetailItemsWithPartInfoQueryHandler(IStorageDetailRepository storageDetailRepository)
    {
        this.storageDetailRepository = storageDetailRepository;
    }

    public async Task<PaginatedList<StorageDetailItemWithPartInfoDto>> Handle(GetStorageDetailItemsWithPartInfoQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Filter.OutboundJobNo) || request.Filter.LineItem <= 0 || request.Filter.WhsCode == null)
            throw new RequiredFilterException("Invalid search criteria - Please enter at least the Outbound JobNo and LineItem and WHSCode for this view");

        return await storageDetailRepository.GetStorageDetailWithPartsForOutJobNoAndLine(request.Pagination, request.Filter, request.OrderByExpression, request.OrderByDescending);
    }
}
