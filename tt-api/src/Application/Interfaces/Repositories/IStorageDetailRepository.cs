using Application.Common.Models;
using Application.UseCases.Storage.Queries.GetPalletsForILogStockDiscrepancyReport;
using Application.UseCases.Storage.Queries.GetILogInboundPalletsWithTypeQuery;
using Application.UseCases.Storage.Queries.GetStorageDetailItems;
using Application.UseCases.StorageDetails;
using Domain.Entities;
using System.Collections.Generic;
using Application.UseCases.Registration.Commands.UpdateLocation;
using Domain.ValueObjects;

namespace Application.Interfaces.Repositories;

public interface IStorageDetailRepository
{
    StorageDetailPaginatedList GetStorageDetailItems(PaginationQuery pagination, StorageDetailItemDtoFilter filter, string? orderBy, bool orderByDescending);
    Task<PaginatedList<StorageDetailItemWithPartInfoDto>> GetStorageDetailWithPartsForOutJobNoAndLine(PaginationQuery pagination, StorageDetailItemWithPartInfoDtoFilter filter, string? orderBy, bool orderByDescending);
    Pallet? GetPalletDetail(string palletId);
    Task Update(Pallet pallet);
    string? GetLastPIDNumber(string prefix);
    void AddNewPIDCode(string newPid, DateTime createdDate);
    Task AddNewPallet(Pallet newPallet, string origPalletId);
    IEnumerable<ILogInboundPalletWithTypeDto> GetILogInboundPalletsWithType(ILogInboundPalletsWithTypeDtoFilter filter, int iLogInboundCategoryId);
    IEnumerable<StockDiscrepancyReportPalletDto> GetPalletsForILogStockDiscrepancyReport(StockDiscrepancyReportPalletDtoFilter filter, int iLogStorageCategoryId);
    List<ILogStockSynchronizationPalletDto> GetILogStockSynchronizationData(string[] WHSCodes, int iLogStorageCategoryId);
    List<Pallet> GetPallets(string[] pids);
    List<StorageStatus> GetPalletStatusesOnLocationForRegistrationUpdateLocation(string locationCode, string locationWhsCode);
    int GetPalletCountOnLocationForRegistrationToggleActiveLocation(string locationCode, string locationWhsCode);
    List<string> GetPidsInILog(string[] pids, string[] whsCodes, int storageCategoryId);
}
