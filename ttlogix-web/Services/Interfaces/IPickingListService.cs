using ServiceResult;
using System.Collections.Generic;
using System.Threading.Tasks;
using TT.Core.QueryFilters;
using TT.Services.Models;

namespace TT.Services.Interfaces
{
    public interface IPickingListService
    {
        Task<IEnumerable<PickingListSimpleDto>> GetPickingListWithUOM(string jobNo, int? lineItem);
        Task<IList<string>> GetPickingDataToDownload(PickingListToDownloadQueryFilter queryFilter);
        Task<bool> HasPickingLists(string jobNo, int lineItem);
        Task<Result<bool>> AutoAllocate(AllocationDto dto);
        Task<Result<bool>> AllocatePickingListBatch(IEnumerable<PickingListAllocateDto> pickingList);
        Task<Result<bool>> UnAllocateBatch(IList<UndoAllocationDto> unAllocateData);
    }
}
