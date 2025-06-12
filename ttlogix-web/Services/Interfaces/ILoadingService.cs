using ServiceResult;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TT.Core.QueryFilters;
using TT.Services.Models;

namespace TT.Services.Interfaces
{
    public interface ILoadingService
    {
        Task<LoadingListDto> GetLoadingList(LoadingListQueryFilter queryFilter);
        Task<IEnumerable<LoadingEntryDto>> GetLoadingEntryList(string customerCode, string warehouse);
        Task<LoadingDto> GetLoading(string jobNo);
        Task<IEnumerable<LoadingDetailDto>> GetLoadingDetails(string jobNo);
        Task<Result<bool>> SetTruckArrival(string jobNo);
        Task<Result<bool>> SetTruckDeparture(string jobNo);
        Task<Result<LoadingDto>> CreateLoading(LoadingAddDto loadingDto, string userCode);
        Task<Result<LoadingDto>> CreateLoadingFromOutbound(AddLoadingFromOutboundDto loadingDto, string userCode, 
            string warehouse);
        Task<Result<LoadingDto>> UpdateLoading(string jobNo, LoadingDto loadingDto, string userCode);
        Task<IEnumerable<string>> GetBondedStockJobNosWithoutCommInv(string jobNo);
        Task<Result<bool>> ConfirmLoading(string jobNo, string userCode);
        Task<Result<LoadingDto>> CancelLoading(string jobNo, string userCode);
        Task<Result<bool>> DeleteLoadingDetails(DeleteLoadingDetailDto data, string userCode);
        Task<Result<bool>> CreateLoadingDetails(IEnumerable<string> entries, string jobNo, string userCode, 
            string warehouse);
        Task<Result<bool>> SetAllowForDispatch(string jobNo, bool allow, string name);
        Task<Stream> LoadingReport(string whsCode, string jobNo);
        Task<Result<Stream>> DeliveryDocketReport(string whsCode, string jobNo);
        Task<Result<Stream>> LoadingPickingInstructionReport(string whsCode, string jobNo, string userCode);
        Task<Result<Stream>> OutboundReport(string whsCode, string jobNo);
        Task<Result<Stream>> DeliveryDocketCombinedReport(string whsCode, string jobNo);
    }
}
