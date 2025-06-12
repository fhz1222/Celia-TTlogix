using ServiceResult;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TT.Core.Entities;
using TT.Core.QueryFilters;
using TT.Core.QueryResults;
using TT.Services.Models;
using TT.Services.Models.ModelEnums;

namespace TT.Services.Interfaces
{
    public interface IEKanbanService
    {
        Task<EKanbanListDto> GetEKanbanListForEurope(EKanbanListQueryFilter filter);
        Task<bool> HasEkanbanForJobNo(string jobNo);
        Task<IEnumerable<EKanbanPartsStatusDto>> GetEKanbanPartsStatusByOwnershipEHP(string orderNo);
        Task<IEnumerable<EKanbanPartsStatusDto>> GetEKanbanPartsStatusForEKanbanCPart(string orderNo);
        Task<IEnumerable<EKanbanPartsStatusDto>> GetEKanbanPartsStatusByOwnership(string orderNo);
        Task<IEnumerable<EKanbanPartsStatusDto>> GetEKanbanPartsStatusForCPart(string orderNo);
        Task<IEnumerable<EKanbanPartsStatusDto>> GetEKanbanPartsStatusForCPartWithoutExt(string orderNo);
        Task<Result<string>> CreateEKanbanManual(string jobNo, string customerCode, ManualType manualType);
        Task<IList<string>> GetEKanbanDataToDownload(string orderNumber);
        Task<Result<bool>> CheckEKanbanFulfillable(IEnumerable<string> orderNos);
        Task<Result<EKanbanHeader>> GetEKanbanHeaderForImport(string orderNo, string whsCode);
        Task<Result<Tuple<SupplierMaster, IEnumerable<EKanbanDetailDistinctProductCodeQueryResult>>>> GetSupplierMasterForEKanbanImport(
            string orderNo, string factoryId, string instruction);
        Task<Result<IEnumerable<EKanbanPartsStatusDto>>> EKanbanCheck(IEnumerable<string> orderNos);
    }
}
