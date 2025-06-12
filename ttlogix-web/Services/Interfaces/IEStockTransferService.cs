using ServiceResult;
using System.Collections.Generic;
using System.Threading.Tasks;
using TT.Core.Entities;
using TT.Core.QueryFilters;
using TT.Services.Models;

namespace TT.Services.Interfaces
{
    public interface IEStockTransferService
    {
        Task<EStockTransferListDto> GetEStockTransferList(EStockTransferListQueryFilter filter);
        Task<Result<EStockTransferHeader>> GetEStockTransferHeaderForImport(string orderNo, string whsCode);
        Task<IEnumerable<EStockTransferPartsStatusDto>> GetEStockTransferPartsStatusByOwnership(string orderNo);
        Task<bool> HasAnyEStockTransferDiscrepancy(string jobNo);
        Task<Result<IEnumerable<EStockTransferPartsStatusDto>>> EStockTransferCheck(IEnumerable<string> orderNos);
    }
}
