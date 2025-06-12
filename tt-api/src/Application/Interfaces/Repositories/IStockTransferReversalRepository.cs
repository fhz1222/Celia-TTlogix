using Application.Common.Models;
using Application.Interfaces.Utils;
using Application.UseCases.StockTransferReversalItems;
using Application.UseCases.StockTransferReversalItems.Commands.AddNewStockTransferReversalItemCommand;
using Application.UseCases.StockTransferReversals;
using Application.UseCases.StockTransferReversals.Commands.CompleteStockTransferReversalCommand;
using Application.UseCases.StockTransferReversals.Queries.GetStockTransferReversals;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IStockTransferReversalRepository : IJobNumberSource
{
    PaginatedList<StockTransferReversalDto> GetStockTransferReversals(PaginationQuery pagination, GetStockTransferReversalsDtoFilter filter, string? orderBy, bool orderByDescending);
    Task<StockTransferInfo?> GetStockTransferInfo(string jobNo);
    Task AddNew(StockTransferReversal newStockTransferReversal);
    PaginatedList<ReversibleStockTransferDto> GetReversibleStockTransfers(PaginationQuery pagination, string whsCode, string? stfJobNo, DateTime? newerThan);
    Task<StockTransferReversal?> GetStockTransferReversal(string jobNo);
    Task<bool> AnyStockTransferReversalDetailsExists(string jobNo);
    Task Update(StockTransferReversal updated);
    Task<string?> GetCustomerName(string customerCode, string whsCode);
    List<StockTransferReversalItemDto> GetStockTransferReversalItems(string jobNo, string? orderBy, bool orderByDescending);
    Task DeleteDetail(string jobNo, string PID);
    int GetDetailsCount(string jobNo);
    bool DetailExists(string jobNo, string PID);
    bool OutstandingReversalExistsForPID(string PID);
    Task<StockTransferDetailInfo?> GetStockTransferDetailInfo(string jobNo, string PID);
    Task AddNewDetail(StockTransferReversalDetail newStockTransferReversalDetail);
    List<ReversibleStockTransferItemDto> GetReversibleStockTransferItems(string stfJobNo, string? orderBy, bool orderByDescending);
    List<StockTransferReversalDetail> GetStockTransferReversalDetails(string jobNo);
    Task<List<StockTransferReversalSummary>> GetStockTransferReversalSummary(string jobNo);
    string GetSupplierParadigm(string supplierId, string customerCode);
}
