using Microsoft.AspNetCore.Mvc;
using ServiceResult;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TT.Common;
using TT.Core.QueryFilters;
using TT.Services.Models;

namespace TT.Services.Interfaces
{
    public interface IStockTransferService
    {
        Task<StockTransferListDto> GetStockTransferList(StockTransferListQueryFilter queryFilter);
        Task<StockTransferDto> GetStockTransfer(string jobNo);
        Task<IEnumerable<StockTransferDetailDto>> GetStockTransferDetailList(string jobNo);
        Task<IEnumerable<StockTransferSummaryDto>> GetStockTransferSummaryList(string jobNo);
        Task<Result<StockTransferDto>> CreateStockTransfer(string customerCode, string userCode, string whsCode);
        Task<Result<StockTransferDto>> UpdateStockTransfer(string jobNo, StockTransferDto stDto);
        Task<Result<string>> ImportEKanbanEUCPart(string orderNo, string whsCode, string name);
        Task<Result<string>> ImportEKanbanEUCPartMulti(IEnumerable<string> orderNumbers, string whsCode, string userCode);
        Task<Result<string>> ImportEStockTransfer(string orderNo, string whsCode, string userCode);
        Task<Result<string>> ImportEStockTransferMulti(IEnumerable<string> orderNumbers, string whsCode, string userCode);
        Task<Result<bool>> AddStockTransferDetailByPID(StockTransferDetailByPIDDto dto, string userCode);
        Task<Result<bool>> DeleteStockTransferDetailByPID(StockTransferDetailByPIDDto dto);
        Task<Result<bool>> DeleteStockTransferDetail(string jobNo, int lineItem);
        Task<Result<bool>> Cancel(string jobNo, string userCode);
        Task<Result<bool>> Complete(string jobNo, string userCode);
        Task<Result<bool>> SplitByInboundDate(string jobNo, string name);
        Task<Stream> StockTransferReport(string whsCode, string jobNo);
        Task<string> StockTransferReportFileName(string jobNo);
        Task<Result<Stream>> DownloadEDTToCSV(string jobNo);
        AllowedSTFImportMethodsDto GetAllowedSTFImportMethods();
    }
}
