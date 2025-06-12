using ServiceResult;
using System.Collections.Generic;
using System.Threading.Tasks;
using TT.Core.QueryFilters;
using TT.Services.Models;

namespace TT.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<CustomerInventoryProductCodeMapDto> GetCustomerInventoryProductCodeMap(string customerCode);
        Task<CustomerInventoryControlCodeMapDto> GetCustomerInventoryControlCodeMap(string customerCode);
        Task<IEnumerable<PartMasterBySupplierDto>> GetPartMasterListBySupplier(string customerCode, string supplierID);
        Task<PartMasterListDto> GetPartMasterList(PartMasterListQueryFilter queryFilter);
        Task<PartMasterDto> GetPartMaster(string customerCode, string productCode1, string supplierID);
        Task<IEnumerable<UOMWithDecimalDto>> GetUOMListWithDecimal(string customerCode);
        Task<Result<PartMasterDto>> UpdatePartMaster(PartMasterDto partMasterDto);
        Task<Result<PartMasterDto>> CreatePartMaster(PartMasterDto partMasterDto, string userCode);
        Task<Result<UnloadingPointChoiceDto>> GetUnloadingPointChoice(string customerCode, string supplierID);
        Task<Result<IEnumerable<PalletTypeDto>>> GetPalletTypes();
        Task<Result<IEnumerable<ELLISPalletTypeDto>>> GetELLISPalletTypes();
    }
}
