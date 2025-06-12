using Application.Common.Models;
using Application.Interfaces.Utils;
using Application.UseCases.InboundReversalItems;
using Application.UseCases.InboundReversalItems.Commands.AddNewInboundReversalItemCommand;
using Application.UseCases.InboundReversalItems.Queries.GetInboundReversalItems;
using Application.UseCases.InboundReversalItems.Queries.GetReversibleInboundItems;
using Application.UseCases.InboundReversals;
using Application.UseCases.InboundReversals.Commands.CompleteInboundReversalCommand;
using Application.UseCases.InboundReversals.Queries.GetInboundReversals;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IInboundReversalRepository : IJobNumberSource
{
    Task<InboundInfo?> GetInboundInfo(string inJobNo);
    Task AddNewInboundReversal(InboundReversal newInboundReversal);
    PaginatedList<InboundReversalDto> GetInboundReversals(PaginationQuery pagination, GetInboundReversalsDtoFilter filter, string? orderBy, bool orderByDescending);
    PaginatedList<ReversibleInboundDto> GetReversibleInbounds(PaginationQuery pagination, string whsCode, string? inJobNo, DateTime? newerThan);
    Task<InboundReversal?> GetInboundReversal(string jobNo);
    Task<bool> AnyInboundReversalDetailsExists(string jobNo);
    Task UpdateInboundReversal(InboundReversal inboundReversal);
    Task<string?> GetCustomerName(string customerCode, string whsCode);
    Task<string?> GetSupplierName(string supplierId, string factoryId);
    IEnumerable<ReversibleInboundItemDto> GetReversibleInboundItems(string inJobNo, GetReversibleInboundItemsDtoFilter filter, string? orderBy, bool orderByDescending);
    Task<PIDInfo?> GetPIDInfo(string PID);
    Task AddNewInboundReversalDetail(InboundReversalDetail newInboundReversalDetail);
    bool OutstandingInboundReversalExistsForPID(string PID);
    bool InboundReversalDetailExists(string jobNo, string PID);
    Task<InboundReversalDetail?> GetInboundReversalDetail(string jobNo, string PID);
    Task DeleteInboundReversalDetail(string jobNo, string PID);
    int GetInboundReversalDetailsCount(string jobNo);
    IEnumerable<InboundReversalItemDto> GetInboundReversalItems(string jobNo, GetInboundReversalItemsDtoFilter filter, string? orderBy, bool orderByDescending);
    List<InboundReversalDetail> GetInboundReversalDetails(string jobNo);
    Task<List<InboundReversalSummary>> GetInboundReversalSummary(string jobNo);
    Task<List<InboundReversalSummaryByProduct>> GetInboundReversalSummaryByProduct(string jobNo);
    string GetSupplierParadigm(string supplierId, string customerCode);
}
