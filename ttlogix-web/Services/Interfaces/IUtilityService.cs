using ServiceResult;
using System.Threading.Tasks;
using TT.Core.Enums;

namespace TT.Services.Interfaces
{
    public interface IUtilityService
    {
        Task<Result<string>> GenerateJobNo(JobType jobType);
        int GetAutoNum(AutoNumTable tableName, string jobNo, int? lineItem = null);
        Task<string> GetNextPIDNumber(bool addToContext = true);
        Task<string> GetNextPIDNumber(string lastPIDCode, bool addToContext = true);
        Task<string> GetNextOrderNo(string prefix, int suffixLength);
        Task<string> GetNextOrderNoForStockTransfer(string prefix, int suffixLength);
        Task<string> GetNextGroupPIDNumber();
        bool ValidateUnloadingPointId(int? id, string CustomerCode);
        bool ValidatePalletTypeId(int id);
        bool ValidateELLISPalletTypeId(int id);
    }
}
