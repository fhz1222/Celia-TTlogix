using ServiceResult;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TT.Core.QueryFilters;
using TT.Services.Models;

namespace TT.Services.Interfaces
{
    public interface IInboundService
    {
        Task<InboundListDto> GetInboundList(InboundListQueryFilter queryFilter);
        Task<InboundDto> GetInbound(string jobNo);
        Task<IEnumerable<InboundDetailDto>> GetInboundDetailList(string jobNo);
        Task<Result<string>> CreateInboundManual(InboundManualDto inboundDto, string userCode);
        Task<Result<InboundDto>> UpdateInbound(string jobNo, InboundDto inboundDto, string userCode);
        Task<Result<bool>> CreateInboundDetail(InboundDetailEntryAddDto inboundDetail, string whsCode, string userCode);
        Task<Result<InboundDetailDto>> UpdateInboundDetail(InboundDetailEntryModifyDto inboundDetail, string whsCode,
            string userCode);
        Task<ASNListDto> GetASNListToImport(ASNListQueryFilter filter);
        Task<IEnumerable<ASNDetailSimpleDto>> GetASNDetails(string asnNo);
        Task<Result<string>> ImportASN(string aSNNo, string whsCode, string userCode);
        Task<Result<string[]>> ImportFile(Stream file, string whsCode, string customerCode, string supplierID,
            string userCode);
        Task<Result<bool>> CancelInbound(string jobNo, string userCode);
        Task<IEnumerable<InboundIDTListItemDto>> GetIDT(string jobNo);
        Task<Result<bool>> IncreasePkgQty(string jobNo, int lineItem, decimal qty, string userCode);
        Task<Result<bool>> RemovePIDs(RemovePIDsDto data, string userCode);
        Task<Stream> LocationReport(string whsCode, string jobNo);
        Task<Stream> InboundReport(string whsCode, string jobNo);
        Task<Result<Stream>> DiscrepancyReport(string whsCode, string jobNo);
        Task<Stream> WarehouseInNoteReport(string whsCode, string jobNo);
        Task<Stream> GetOutstandingInboundsXlsReport(string whsCode);
    }
}
