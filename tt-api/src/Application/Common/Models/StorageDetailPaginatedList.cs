using Application.UseCases.StorageDetails;

namespace Application.Common.Models;

public class StorageDetailPaginatedList : PaginatedList<StorageDetailItemDto>
{
    public decimal TotalQty { get; set; }
    public StorageDetailPaginatedList(List<StorageDetailItemDto> items, int count, int pageNumber, int pageSize, decimal totalQty) 
        : base(items, count, pageNumber, pageSize)
    {
        TotalQty = totalQty;
    }
}
