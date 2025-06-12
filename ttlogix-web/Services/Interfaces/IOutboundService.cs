using ServiceResult;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TT.Core.Entities;
using TT.Core.QueryFilters;
using TT.Core.QueryResults;
using TT.Services.Models;

namespace TT.Services.Interfaces
{
    public interface IOutboundService
    {
        Task<OutboundListDto> GetOutboundList(OutboundListQueryFilter filter);
        Task<IEnumerable<OutboundDetailDto>> GetOutboundDetailList(string jobNo);
        Task<IEnumerable<OutboundDetailDto>> GetOutboundDetailWithReceivedQtyList(string jobNo);
        Task<OutboundDto> GetOutbound(string jobNo);
        Task<IEnumerable<OutboundPickableListDto>> GetOutboundPickableList(OutboundPickableListQueryFilter filter);
        Task<Result<string>> CreateOutboundManual(OutboundManualDto outboundDto, string userCode);
        Task<Result<Outbound>> UpdateOutbound(string jobNo, OutboundDto outboundDto);
        Task<Result<bool>> CancelOutbound(string jobNo, string userCode);
        Task<Result<bool>> UpdateOutboundStatus(string jobNo);
        Task<Result<string>> ImportEKanbanEUCPart(string orderNo, string factoryId, string whsCode, string userCode);
        Task<Result<bool>> CompleteOutboundEurope(IEnumerable<string> jobNos, string userCode, bool withTransaction = true);
        Task<Result<bool>> CompleteOutboundReturn(IEnumerable<string> jobNos, string userCode, bool withTransaction = true);
        Task<Result<bool>> CompleteOutboundManual(IEnumerable<string> jobNos, string userCode, bool withTransaction = true);
        Task<Result<bool>> CargoInTransit(IEnumerable<string> jobNos, string name, bool withTransaction = true);
        Task<Result<bool>> CancelAllocation(CancelAllocationDto data);
        Task<Result<IEnumerable<string>>> SplitOutbound(SplitOutboundDto data, string userCode);
        Task<Result<IEnumerable<string>>> SplitOutboundByDateOrInJobNo(string jobNo, bool splitByInboundDate, string userCode);
        Task<Result<IEnumerable<string>>> SplitOutboundByOwnership(string jobNo, string userCode);
        Task<Result<bool>> ReleaseBondedStock(OutboundDto outbound, string userCode);
        Task<Result<byte[]>> GetOutboundQRCodeImage(string jobNo, string userCode);
        Task<Result<bool>> DeleteOutboundDetail(string jobNo, int lineItem);
        Task<Result<bool>> CompleteDiscrepancyOutbound(string jobNo, string name);
        Task<Result<int>> AddNewOutboundDetail(OutboundDetailAddDto data, string userCode);
        Task<Result<bool>> UndoPicking(string outJobNo, IEnumerable<string> PIDs);
        Task<Result<bool>> CompleteWHSTransfer(IEnumerable<string> jobNos, string userCode);
        Task<Stream> PickingListReport(string whsCode, string userCode, string outJobNo);
        Task<Stream> PickingInstructionReport(string whsCode, string userCode, string outJobNo);
        Task<Result<Stream>> OutboundReport(string whsCode, string userCode, string outJobNo);
        Task<Stream> DeliveryDocketWithPIDReport(string whsCode, string userCode, string outJobNo);
        Task<Stream> PackingListReport(string whsCode, string userCode, string outJobNo);
        Task<Stream> DeliveryDocketReport(string whsCode, string userCode, string outJobNo);
        Task<Result<Stream>> DownloadEDTToCSV(string jobNo, string userCode);
        Task<Result<bool>> DispatchWarehouseTransfer(string jobNo, string userCode);
        AllowedOutboundCreationMethodsDto GetAllowedOutboundCreationMethods();
        Task<List<OutboundOrderSummaryQueryResult>> GetOrderSummary(string jobNo);
    }
}
