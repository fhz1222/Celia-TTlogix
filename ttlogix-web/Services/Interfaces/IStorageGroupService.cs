using ServiceResult;
using System.Collections.Generic;
using System.Threading.Tasks;
using TT.Core.QueryFilters;
using TT.Core.QueryResults;
using TT.Services.Models;

namespace TT.Services.Interfaces
{
    public interface IStorageGroupService
    {
        Task<Result<string[]>> CreateGroup(string whsCode, int num, string prefix);
        Task<Result<bool>> DeleteGroup(string GroupID);
        Task<Result<bool>> TransformGroup(string GroupID);
        Task<StorageGroupListDto> GetGroupList(string whsCode, StorageGroupListQueryFilter queryFilter);
        Task<Result<bool>> PrintLabels(string[] GroupIDs, ILabelFactory.LabelType type, string printer, int copies);
        Task<Result<bool>> PrintPIDLabels(string[] GroupIDs, ILabelFactory.LabelType type, string printer);
        Task<IEnumerable<AllocatedStorageDetailSummaryQueryResult>> GetStorageDetails(string GroupID);
        Task<Result<IEnumerable<StorageLabelDto>>> GetStorageLabelsForGIDs(string[] gids);
        Task<Result<IEnumerable<GroupLabelDto>>> GetGroupLabels(string[] gids);
    }
}
