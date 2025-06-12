using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TT.Core.Entities;
using TT.Core.Enums;
using TT.Core.QueryFilters;
using TT.Core.QueryResults;

namespace TT.Core.Interfaces
{
    public interface ITTLogixRepository
    {
        #region Queryable collections

        IQueryable<Outbound> Outbounds();
        IQueryable<Customer> Customers();
        IQueryable<OutboundDetail> OutboundDetails();
        IQueryable<PickingList> PickingLists();
        IQueryable<SupplierMaster> SupplierMasters();
        IQueryable<EKanbanHeader> EKanbanHeaders();
        IQueryable<EKanbanDetail> EKanbanDetails();
        IQueryable<Warehouse> Warehouses();
        IQueryable<Inventory> Inventory();
        IQueryable<PartMaster> PartMasters();
        IQueryable<UOM> UOMs();
        IQueryable<UOMDecimal> UOMDecimals();
        IQueryable<ExternalPID> ExternalPIDs();
        IQueryable<StorageDetail> StorageDetails();

        IQueryable<StorageDetailGroup> StorageDetailGroups();
        IQueryable<Inbound> Inbounds();
        IQueryable<InboundDetail> InboundDetails();
        IQueryable<PriceMaster> PriceMasters();
        IQueryable<Location> Locations();
        IQueryable<EOrder> EOrders();
        IQueryable<PickingListEKanban> PickingListEKanbans();
        IQueryable<PickingAllocatedPID> PickingAllocatedPIDs();
        IQueryable<Loading> Loadings();
        IQueryable<LoadingDetail> LoadingDetails();
        IQueryable<ReportPrintingLog> ReportPrintingLogs();
        IQueryable<ASNHeader> ASNHeaders();
        IQueryable<ASNDetail> ASNDetails();
        IQueryable<EPO> EPOs();
        IQueryable<PackageType> PackageTypes();
        IQueryable<QuarantineLog> QuarantineLogs();
        IQueryable<SupplierItemMaster> SupplierItemMasters();
        IQueryable<FactoryMaster> FactoryMasters();
        IQueryable<Country> Countries();
        IQueryable<SupplierDetail> SupplierDetails();
        IQueryable<LabelPrinter> LabelPrinters();
        IQueryable<User> Users();
        IQueryable<AccessGroup> AccessGroups();
        IQueryable<SystemModule> SystemModules();
        IQueryable<AccessRight> AccessRights();
        IQueryable<StockTransfer> StockTransfers();
        IQueryable<StockTransferDetail> StockTransferDetails();
        IQueryable<EStockTransferHeader> EStockTransferHeaders();
        IQueryable<ILogLocationCategory> ILogLocationCategories();

        #endregion

        #region queries
        IQueryable<T> GetOutboundList<T>(OutboundListQueryFilter filter) where T : OutboundListQueryResult, new();
        IQueryable<T> GetStockTransferList<T>(StockTransferListQueryFilter filter) where T : StockTransferListQueryResult, new();
        IQueryable<T> GetEKanbanList<T>(EKanbanListQueryFilter filter) where T : EKanbanListQueryResult, new();
        IQueryable<T> GetEStockTransferList<T>(EStockTransferListQueryFilter filter) where T : EStockTransferListQueryResult, new();
        IQueryable<T> GetLoadingList<T>(LoadingListQueryFilter filter) where T : LoadingListQueryResult, new();
        IQueryable<T> GetInboundList<T>(InboundListQueryFilter filter) where T : InboundListQueryResult, new();
        IQueryable<T> GetStorageDetailGroupList<T>(string whsCode, StorageGroupListQueryFilter filter) where T : StorageDetailGroup, new();
        IQueryable<T> GetASNListToImport<T>(ASNListQueryFilter filter) where T : ASNListQueryResult, new();
        IQueryable<T> GetPartMasterList<T>(PartMasterListQueryFilter queryFilter) where T : PartMasterListQueryResult, new();
        IQueryable<T> GetUsersList<T>(UserListQueryFilter queryFilter) where T : UserListQueryResult, new();

        Task<IEnumerable<OutstandingInboundForReportQueryResult>> GetOutstandingInboundForReport(string whsCode);

        Task<IEnumerable<T>> GetCountries<T>() where T : CountryListQueryResult, new();
        Task<IEnumerable<T>> GetPackageTypes<T>() where T : PackageTypeListQueryResult, new();
        Task<int> GetNoOfPalletsForLoading(string jobNo);
        Task<int> GetNoOfPalletsForOutbound(string jobNo);
        Task<IList<StorageDetailExtendedQueryResult>> GetStorageDetailListEuro(StorageDetailExtendedQueryFilter filter);
        Task<IList<string>> GetStoragePIDsForEKanban(StorageDetailExtendedQueryFilter filter);
        Task<IEnumerable<KanbanListGroupQueryResult>> GetKanbanListGroupByProductCode(string orderNo);
        Task<IList<EKanbanDetail>> GetEKanbanDetailForPicking(EKanbanForPickingQueryFilter filter);
        Task<IEnumerable<T>> GetPickingListWithUOM<T>(string jobNo, int? lineItem) where T : PickingListSimpleQueryResult, new();
        Task<IEnumerable<T>> GetOutboundPickableList<T>(OutboundPickableListQueryFilter filter) where T : OutboundPickableListQueryResult, new();
        Task<IEnumerable<T>> GetLoadingEntryListFromOutbound<T>(string customerCode, string whsCode, bool isSAP, IEnumerable<string> outboundJobNos) where T : LoadingEntryListQueryResult, new();
        Task<IEnumerable<T>> GetLoadingEntryList<T>(string customerCode, string whsCode, bool isSAP, IEnumerable<string> orderNos) where T : LoadingEntryListQueryResult, new();

        EKanbanSummaryQueryResult GetSummaryQuantitiesFromEKanban(string refNo, string productCode, string supplierId);
        Task<IEnumerable<OutboundDetailPickingQueryResult>> GetOutboundDetailPickingResultList(string jobNo);
        Task<IEnumerable<PickingList>> GetPickingLists(IEnumerable<string> jobNos, IEnumerable<int> lineItems = null);
        Task<(double BalanceQty, long BalancePkg)?> GetInventoryLastTransactionBalance(string customerCode, string productCode);
        Task<(double BalanceQty, long BalancePkg)?> GetInventoryLastTransactionPerWHSBalance(string customerCode, string productCode, string whsCode);
        Task<decimal?> GetInventoryLastTransactionPerSupplierBalance(string customerCode, string productCode, string supplierId, Ownership ownership);
        Task<IEnumerable<OutboundBillingDetailQueryResult>> GetOutboundBillingDetail(string orderNo);
        Task<IEnumerable<T>> GetUOMListWithDecimal<T>(string customerCode) where T : UOMWithDecimalQueryResult, new();
        Task<IEnumerable<PickingListForCargoOutCheckingQueryResult>> GetPickingListForCargoOutChecking(string jobNo);
        Task<IEnumerable<T>> GetOutboundDetailList<T>(string jobNo) where T : OutboundDetailQueryResult, new();
        Task<IEnumerable<T>> GetStockTransferDetailList<T>(string jobNo) where T : StockTransferDetailQueryResult, new();
        Task<IEnumerable<T>> GetStockTransferSummaryList<T>(string orderNo) where T : StockTransferSummaryQueryResult, new();
        Task<IEnumerable<T>> GetLoadingDetailList<T>(string jobNo) where T : LoadingDetailQueryResult, new();
        Task<IEnumerable<T>> GetInboundDetailListWithPrice<T>(string jobNo, int? lineItem = null) where T : InboundDetailQueryResult, new();
        Task<IEnumerable<T>> GetStoragePutawayList<T>(string jobNo, int? lineItem) where T : StorageDetailQueryResult, new();
        Task<IEnumerable<T>> GetPartMasterListBySupplier<T>(string CustomerCode, string supplierID) where T : PartMasterBySupplierQueryResult, new();
        Task<string> GetCurrencyForLoading(string jobNo);
        Task<IEnumerable<OutboundStatus>> GetOutboundStatusesForLoading(string jobNo);
        Task<IEnumerable<string>> GetCycleCountJobNos(IEnumerable<string> productCodes, string whsCode);
        Task<IList<PickingListDataToDownloadQueryResult>> GetPickingListDataToDownload(PickingListToDownloadQueryFilter queryFilter);
        Task<IEnumerable<EStockTransferDetailForFiltersQueryResult>> GetEStockTransferDetailForFilters(string jobNo);
        Task<IEnumerable<StorageDetailWithPartInfoQueryResult>> GetStorageDetailWithPartInfo(StorageDetailQueryFilter filter);
        Task<IEnumerable<SFTStorageDetailWithPartInfoQueryResult>> GetSFTStorageDetailWithPartInfo(SFTStorageDetailQueryFilter filter);
        Task<IEnumerable<T>> GetDistinctStorageSupplierList<T>(string customerCode, string whsCode) where T : SupplierQueryResult, new();
        Task<IEnumerable<T>> GetDistinctStorageInJobNoList<T>(string customerCode, string supplierId, string whsCode) where T : InJobNoQueryResult, new();
        Task<IEnumerable<ASNDetailWithSPQQueryResult>> GetASNDetailWithSPQList(string asnNo);
        Task<IEnumerable<InboundQtyByProductCodeQueryResult>> GetInboundDetailGroupByProductCode(string jobNo);
        Task<IEnumerable<T>> GetEKanbanDetailDistinctProductCodeList<T>(string orderNo, string productCode, string supplierId) where T : EKanbanDetailDistinctProductCodeQueryResult, new();
        Task<IEnumerable<T>> GetEStockTransferDistinctProductCodeList<T>(string orderNo, string productCode, string supplierId) where T : EKanbanDetailDistinctProductCodeQueryResult, new();
        Task<IEnumerable<StorageDetail>> GetExpiredStorageDetails(string whsCode, string supplierId, string factoryId, IEnumerable<String> productCodes);
        Task<decimal?> GetOutboundDetailUnallocatedQty(string customerCode, string supplierId, string productCode, string whsCode);
        Task<IEnumerable<AllocatedStorageDetailSummaryQueryResult>> GetAllocatedStorageDetailSummaryList(AllocatedStorageDetailSummaryQueryFilter filter);
        Task<IEnumerable<AllocatedStorageDetailSummaryQueryResult>> GetStorageDetailSummaryListForGroup(string groupID);
        Task<bool> HasAnyEStockTransferDiscrepancy(string jobNo);
        Task<string> GetLastPIDCode(string match);

        Task<string> GetLastGroupPIDCode(string match);
        Task<string> GetLastEKanbanOrderNo(string match);
        Task<string> GetLastEStockTransferOrderNo(string match);

        Task<StorageDetail> GetStorageDetailForFilter(StorageQueryFilter filter);
        Task<IEnumerable<StorageDetail>> GetStorageDetailForLocationType(LocationType locationType, string inJobNo);
        Task<IEnumerable<T>> GetOutboundDetailWithReceivedQtyList<T>(string jobNo) where T : OutboundDetailQueryResult, new();
        string GetSuppliers(string jobNo);
        Task<IEnumerable<WHSTransferSummaryQueryResult>> GetWHSTransferSummaryList(string jobNo);
        Task<IEnumerable<string>> GetBondedStockJobNosWithoutCommInv(string jobNo);
        Task<IEnumerable<OutboundStatus>> GetDistinctLoadingOutboundList(string jobNo);
        Task<IEnumerable<ReportPrintingLog>> GetLastReportPrintingLogs(string jobNo);
        Task<IEnumerable<T>> GetInboundIDTList<T>(string jobNo) where T : InboundIDTListItemQueryResult, new();
        Task<T> GetInboundExtendedAsync<T>(string jobNo) where T : InboundWithExtendedDataQueryResult, new();
        Task GetOutboundEDTDataAsync(string jobNo, Action<IDataReader> action);
        Task GetStockTransferEDTDataAsync(string jobNo, Action<IDataReader> action);
        Task<IEnumerable<string>> GetSystemModuleNamesForGroup(string groupCode);
        Task<IEnumerable<T>> GetAccessGroups<T>(AccessGroupFilter filter) where T : AccessGroupSimpleQueryResult, new();
        Task<IEnumerable<StockTransferDetailGroupListQueryResult>> GetStockTransferDetailGroupList(string jobNo);
        Task<StockTransferTotalValueQueryResult> GetStockTransferTotalValueQueryResult(string jobNo);
        Task<IEnumerable<UnloadingPoint>> GetUnloadingPoints(string customerCode);
        Task<int?> GetDefaultUnloadingPointId(string customerCode, string supplierId);
        Task<IEnumerable<T>> GetPalletTypes<T>() where T : PalletType, new();
        Task<IEnumerable<T>> GetELLISPalletTypes<T>() where T : ELLISPalletType, new();

        #endregion

        #region get entity
        Task<Outbound> GetOutboundAsync(string jobNo);
        Task<OutboundDetail> GetOutboundDetailAsync(string jobNo, int lineItem);
        Task<PickingList> GetPickingListAsync(string jobNo, int lineItem, int seqNo);
        Task<PickingListEKanban> GetPickingListEKanbanAsync(string jobNo, int lineItem, int seqNo);
        Task<PickingAllocatedPID> GetPickingAllocatedPIDAsync(string jobNo, int lineItem, int seqNo);
        Task<PartMaster> GetPartMasterAsync(string customerCode, string supplierId, string productCode);
        Task<Inbound> GetInboundAsync(string inJobNo);
        Task<InboundDetail> GetInboundDetailAsync(string jobNo, int lineItem);
        Task<Inventory> GetInventoryAsync(string customerCode, string supplierID, string productCode, string wHSCode, Ownership ownership);
        Task<EKanbanHeader> GetEKanbanHeaderAsync(string orderNo);
        Task<IEnumerable<EKanbanHeader>> GetEKanbanHeadersAsync(params string[] orderNos);
        Task<EStockTransferHeader> GetEStockTransferHeaderAsync(string orderNo);
        Task<EStockTransferDetail> GetEStockTransferDetailAsync(string orderNo, string productCode, string serialNo);
        Task<EKanbanDetail> GetEKanbanDetailAsync(string orderNo, string productCode, string serialNo);

        Task<StorageDetailGroup> GetStorageDetailGroupAsync(string groupID);
        Task<StorageDetail> GetStorageDetailAsync(string pid);
        Task<OutboundQRCode> GetOutboundQRCodeAsync(string jobNo);
        Task<InventoryControl> GetInventoryControlAsync(string customerCode);
        Task<ProductCode> GetProductCodeAsync(string code);
        Task<ControlCode> GetControlCodeAsync(string code);
        Task<SupplierMaster> GetSupplierMasterAsync(string factoryId, string supplierId);
        Task<IEnumerable<SupplierMaster>> GetSupplierMasterListAsync(string factoryId);
        Task<PriceMaster> GetPriceMasterAsync(string customerCode, string supplierId, string productCode);
        Task<AccessLock> GetAccessLockAsync(string jobNo);
        Task<PickingListAllocatedPID> GetPickingListAllocatedPIDAsync(string jobNo, int lineItem, int seqNo);
        Task<IEnumerable<Outbound>> GetLoadingOutboundList(string jobNo, IEnumerable<OutboundStatus> statuses, bool invertStatusFilter = false);
        IQueryable<Outbound> GetLoadingOutboundQuery(string loadingJobNo);
        Task<Owner> GetOwnerAsync(string code);
        Task<Loading> GetLoadingAsync(string jobNo);
        Task<LoadingDetail> GetLoadingDetailAsync(string jobNo, string orderNo);
        Task<JobCode> GetJobCodeAsync(int code);
        Task<Location> GetLocationAsync(string code, string whsCode);
        Task<Customer> GetCustomerAsync(string code, string whsCode);
        Task<ASNHeader> GetASNHeaderAsync(string asnNo);
        Task<ASNDetail> GetASNDetailAsync(string asnNo, int lineItem);
        Task<SupplierItemMaster> GetSupplierItemMasterAsync(string factoryID, string supplierID, string productCode);
        Task<SunsetExpiredAlert> GetSunsetExpiredAlertAsync(string factoryID, string supplierID, string productCode);
        Task<ItemMaster> GetItemMasterAsync(string factoryID, string supplierID, string productCode);
        Task<User> GetUserAsync(string code);
        Task<AccessGroup> GetAccessGroupAsync(string code);
        Task<StockTransfer> GetStockTransferAsync(string jobNo);
        Task<StockTransferDetail> GetStockTransferDetailAsync(string jobNo, int lineItem);

        Task<IEnumerable<CompleteOutboundQueryResult>> GetCompleteOutboundData(IEnumerable<string> jobNos);

        #endregion

        #region add entity
        Task AddOutboundAsync(Outbound entity);
        Task AddOutboundDetailAsync(OutboundDetail entity);
        Task AddEKanbanHeaderAsync(EKanbanHeader entity);
        Task AddEKanbanDetailAsync(EKanbanDetail entity);
        Task AddEStockTransferHeaderAsync(EStockTransferHeader entity);
        Task AddEStockTransferDetailAsync(EStockTransferDetail entity);
        Task AddPickingListEKanbanAsync(PickingListEKanban entity);
        Task AddEOrderAsync(EOrder entity);
        Task AddPickingListAsync(PickingList entity);
        Task AddPickingAllocatedPIDAsync(PickingAllocatedPID entity);
        Task AddInvTransactionAsync(InvTransaction entity, bool saveChanges = true);
        Task AddInvTransactionPerWHSAsync(InvTransactionPerWHS entity, bool saveChanges = true);
        Task AddInvTransactionPerSupplierAsync(InvTransactionPerSupplier entity, bool saveChanges = true);
        Task AddBillingLogAsync(BillingLog entity);
        Task AddOutboundReleaseBondedLogAsync(OutboundReleaseBondedLog entity);
        Task AddOutboundQRCodeAsync(OutboundQRCode entity);
        Task AddPriceMasterAsync(PriceMaster entity);
        Task AddAccessLockAsync(AccessLock entity);
        Task AddPickingListFixLogAsync(PickingListFixLog entity);
        Task AddPIDCodeAsync(PIDCode entity);
        Task BatchAddPIDCodeAsync(IEnumerable<PIDCode> entities);
        Task AddStorageDetailAsync(StorageDetail entity);
        Task BatchAddStorageDetailAsync(IEnumerable<StorageDetail> entities);
        Task AddStorageDetailGroupAsync(StorageDetailGroup entity);
        Task BatchAddExternalPIDAsync(IEnumerable<ExternalPID> entities, bool saveChanges = true);
        Task AddErrorLogAsync(ErrorLog entity);
        Task AddWHSTransferAsync(WHSTransfer entity);
        Task AddWHSTransferDetailAsync(WHSTransferDetail entity);
        Task AddLocationAsync(Location entity);
        Task AddShortfallCancelLogAsync(ShortfallCancelLog entity);
        Task AddLoadingAsync(Loading entity);
        Task AddLoadingDetailAsync(LoadingDetail entity);
        Task AddReportPrintingLogAsync(ReportPrintingLog entity);

        Task AddInboundAsync(Inbound entity);
        Task AddInboundDetailAsync(InboundDetail entity);
        Task BatchAddInboundDetailAsync(IEnumerable<InboundDetail> entity);
        Task AddQuarantineLogAsync(QuarantineLog entity);
        Task AddPartMasterAsync(PartMaster entity);
        Task AddInventoryAsync(Inventory entity);
        Task AddItemMasterAsync(ItemMaster entity);
        Task AddStockTransferAsync(StockTransfer entity);
        Task AddStockTransferDetailAsync(StockTransferDetail entity);
        Task AddSunsetExpiredAlertAsync(SunsetExpiredAlert entity);
        Task AddSupplierItemMasterAsync(SupplierItemMaster entity);
        Task AddUserAsync(User entity);
        Task AddAccessGroupAsync(AccessGroup entity);
        Task BatchAddAccessRightAsync(IEnumerable<AccessRight> entity);
        Task BatchAddStockTransferDetailAsync(IEnumerable<StockTransferDetail> entities);

        #endregion

        #region delete entity
        Task DeleteStorageDetailGroupAsync(StorageDetailGroup entity);
        Task DeleteOutboundAsync(Outbound entity);
        Task DeleteLoadingDetailAsync(LoadingDetail entity);
        Task DeleteOutboundDetailAsync(OutboundDetail entity);
        Task DeleteEKanbanDetailAsync(EKanbanDetail entity);
        Task DeleteEOrderAsync(EOrder entity);
        Task DeletePickingListAsync(PickingList entity);
        Task DeletePickingAllocatedPIDAsync(PickingAllocatedPID entity);
        Task DeleteAccessLockAsync(AccessLock entity);
        Task DeleteAccessLockByComputerName(string ComputerName);
        Task DeleteAllAccessLocksAsync();
        Task DeleteTimeoutedLocksAsync();
        Task DeleteAllAccessRightsAsync(string groupCode);
        Task DeletePickingListAllocatedPIDAsync(PickingListAllocatedPID entity);
        Task DeletePickingListEKanbanAsync(PickingListEKanban entity);
        Task DeleteInboundDetailAsync(InboundDetail entity);
        Task DeleteStockTransferDetailAsync(StockTransferDetail entity);
        Task BatchDeleteStockTransferDetailAsync(IEnumerable<StockTransferDetail> entity);

        #endregion

        #region update
        Task<int> EKanbanHeaderBatchUpdateStatus(EKanbanStatus status, bool updateConfirmationDate, IEnumerable<string> ordernos, EKanbanStatus? currentStatus = null);
        Task<int> EKanbanDetailsBatchUpdateQtyReceived(params string[] orderno);

        #endregion

        Task SaveChangesAsync();
        void ChangeTrackingOff();

        Task ExecuteQueryAsync(string query, Action<IDataReader> action, params KeyValuePair<string, object>[] parameters);
        UnloadingPoint GetUnloadingPoint(int Id);
        PalletType GetPalletType(int Id);
        ELLISPalletType GetELLISPalletType(int Id);
        Task<List<OutboundOrderSummaryQueryResult>> GetOutboundOrderSummary(string outJobNo);
        Task<bool> HasCancelledOrderLines(string orderNo);
    }

}
