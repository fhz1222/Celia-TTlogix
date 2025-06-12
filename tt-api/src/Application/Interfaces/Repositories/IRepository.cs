namespace Application.Interfaces.Repositories;

public interface IRepository
{
    IInventoryRepository Inventory { get; }
    IStorageDetailRepository StorageDetails { get; }
    IAdjustmentItemRepository AdjustmentItems { get; }
    IAdjustmentRepository Adjustments { get; }
    IUtilsRepository Utils { get; }
    ITransactionRepository InventoryTransactions { get; }
    IQuarantineRepository Quarantine { get; }
    IDecantRepository Decant { get; }
    ILocationRepository Locations { get; }
    IRelocationRepository Relocations { get; }
    IPalletTransferRequestsRepository PalletTransferRequests { get; }
    IPickingRepository Picking { get; }
    IILogIntegrationRepository ILogIntegrationRepository { get; }
    IILogBoxRepository ILogBoxes { get; }
    IILogPickingRequestRepository ILogPickingRequests { get; }
    IOutboundRepository Outbounds { get; }
    IInvoiceRequestRepository InvoiceRequests { get; }
    IInboundReversalRepository InboundReversals { get; }
    IBillingLogRepository BillingLogs { get; }
    IStockTransferReversalRepository StockTransferReversals { get; }
    IInboundRepository Inbounds { get; }
    ICustomerRepository Customers { get; }
    ICompanyProfileRepository CompanyProfiles { get; }
    ICountryRepository Countries { get; }
    IMetadataRepository Metadata { get; }
    void BeginTransaction();
    void CommitTransaction();
    void RollbackTransaction();
    void RunInTransaction(Action action);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}