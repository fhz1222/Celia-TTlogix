using ServiceResult;
using System.Collections.Generic;
using System.Threading.Tasks;
using TT.Services.Models;

namespace TT.Services.Interfaces
{
    public interface IStorageService
    {
        Task<bool> HasBondedStock(string outJobNo);
        Task<IEnumerable<StorageDetailDto>> GetStoragePutawayList(string inJobNo, int? lineItem);
        Task<Result<IEnumerable<ExternalQRCodeForInboundDetailDto>>> GetExternalQRStorageLabelsForInbound(string inJobNo,
            string supplierCode, string factoryId);
        Task<Result<bool>> UpdateSellingPrice(IEnumerable<UpdateSellingPriceItemDto> sellingPriceItems);
        Task<Result<bool>> UpdateBuyingPrice(UpdateBuyingPriceItemDto data);
        Task<IEnumerable<StorageDetailWithPartInfoDto>> GetStorageDetailWithPartsInfoList(string outJobNo, int lineItem,
            string whsCode);
        Task<IEnumerable<STFStorageDetailWithPartInfoDto>> GetSTFStorageDetailList(string stockTransferJobNo, string inJobNo,
            string supplierId, string whsCode);
        Task<IEnumerable<SupplierDto>> GetStorageSupplierList(string customerCode, string whsCode);
        Task<IEnumerable<InJobNoDto>> GetStorageInJobNosList(string customerCode, string supplierId, string whsCode);
        Task<Result<bool>> PrintLabels(string[] PID, ILabelFactory.LabelType type, string printer, int copies);
        Task<Result<IEnumerable<StorageLabelDto>>> GetStorageLabels(string[] PID);
    }
}
