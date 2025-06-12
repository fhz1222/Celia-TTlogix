using ServiceResult;
using System.Collections.Generic;
using System.Threading.Tasks;
using TT.Services.Models;

namespace TT.Services.Interfaces
{
    public interface IPriceMasterService
    {
        Task<Result<bool>> UpdatePriceMasterOutbound(IEnumerable<UpdatePriceMasterPickingListDto> pickingListForPriceMasterDtos,
            string customerCode, string jobNo, string userCode);
        Task<Result<bool>> UpdatePriceMasterInbound(IEnumerable<UpdatePriceMasterInboundDetailsDto> inbpundDetailsForPriceMasterDtos,
            string customerCode, string supplierID, string jobNo, string userCode, string currency);
    }
}
