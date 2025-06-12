using Application.Interfaces.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Persistence.PetaPoco;
using System.Data;

namespace Persistence.Repositories;

public class Repository : IRepository
{
    private readonly AppDbContext appDbContext;

    public IInventoryRepository Inventory { get; }

    public IStorageDetailRepository StorageDetails { get; }

    public IAdjustmentItemRepository AdjustmentItems { get; }

    public IAdjustmentRepository Adjustments { get; }
    public IUtilsRepository Utils { get; }
    public ITransactionRepository InventoryTransactions { get; }
    public IQuarantineRepository Quarantine { get; }
    public IDecantRepository Decant { get; }
    public ILocationRepository Locations { get; }
    public IRelocationRepository Relocations { get; }
    public IPalletTransferRequestsRepository PalletTransferRequests { get; }
    public IPickingRepository Picking { get; }
    public IILogIntegrationRepository ILogIntegrationRepository { get; }
    public IILogBoxRepository ILogBoxes { get; }
    public IILogPickingRequestRepository ILogPickingRequests { get; }
    public IOutboundRepository Outbounds { get; }
    public IInvoiceRequestRepository InvoiceRequests { get; }
    public IInboundReversalRepository InboundReversals { get; }
    public IBillingLogRepository BillingLogs { get; }
    public IStockTransferReversalRepository StockTransferReversals { get; }
    public IInboundRepository Inbounds { get; }
    public ICustomerRepository Customers { get; }
    public ICompanyProfileRepository CompanyProfiles { get; }
    public ICountryRepository Countries { get; }
    public IMetadataRepository Metadata { get; }

    private IDbContextTransaction? transaction = null;

    public Repository(
        AppDbContext appDbContext, IPPDbContextFactory factory, IMapper mapper, IServiceProvider provider)
    {
        this.appDbContext = appDbContext;
        Inventory = new InventoryRepository(appDbContext, mapper);
        StorageDetails = new StorageDetailRepository(appDbContext, mapper);
        Adjustments = new AdjustmentRepository(factory, appDbContext, mapper);
        AdjustmentItems = new AdjustmentItemRepository(factory, appDbContext, mapper);
        Utils = new UtilsRepository(factory, mapper);
        InventoryTransactions = new TransactionRepository(appDbContext, mapper);
        Quarantine = new QuarantineRepository(factory, appDbContext, mapper);
        Decant = new DecantRepository(factory, appDbContext, mapper);
        Locations = new LocationRepository(appDbContext);
        Relocations = new RelocationRepository(factory, mapper, appDbContext);
        PalletTransferRequests = new PalletTransferRequestsRepository(factory, appDbContext);
        Picking = new PickingRepository(appDbContext, mapper);
        ILogIntegrationRepository = new ILogIntegrationRepository(factory, appDbContext, mapper);
        ILogBoxes = new ILogBoxRepository(factory, appDbContext);
        ILogPickingRequests = new ILogPickingRequestRepository(appDbContext, mapper);
        Outbounds = new OutboundRepository(appDbContext, mapper);
        InvoiceRequests = new InvoiceRequestRepository(factory, appDbContext, mapper);
        InboundReversals = new InboundReversalRepository(appDbContext, mapper);
        BillingLogs = new BillingLogRepository(appDbContext, mapper);
        StockTransferReversals = new StockTransferReversalRepository(appDbContext, mapper);
        Inbounds = new InboundRepository(appDbContext, mapper);
        Customers = new CustomerRepository(appDbContext, mapper);
        CompanyProfiles = new CompanyProfileRepository(appDbContext, mapper);
        Countries = new CountryRepository(appDbContext, mapper);
        Metadata = new MetadataRepository(appDbContext, mapper, provider);
    }

    public void BeginTransaction()
    {
        if (transaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }
        // TODO set isolation level and timeout
        transaction = appDbContext.Database.BeginTransaction(IsolationLevel.RepeatableRead);
    }

    public void CommitTransaction()
    {
        if (transaction == null)
        {
            throw new InvalidOperationException("No active transation to commit.");
        }
        transaction.Commit();
        transaction = null;
    }
    public void RollbackTransaction()
    {
        if (transaction == null)
        {
            throw new InvalidOperationException("No active transation to rollback.");
        }
        transaction.Rollback();
        transaction = null;
    }

    public void RunInTransaction(Action action)
    {
        BeginTransaction();
        try
        {
            action();
            CommitTransaction();
        }
        catch
        {
            RollbackTransaction();
            throw;
        }
    }

    /// <summary>
    /// Saves all changes made in AppDbContext
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        => await appDbContext.SaveChangesAsync(cancellationToken);
}
