using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TtInventory> TtInventories { get; set; } = null!;
        public virtual DbSet<TtStorageDetail> TtStorageDetails { get; set; } = null!;
        public virtual DbSet<TtInbound> TtInbounds { get; set; } = null!;
        public virtual DbSet<TtInboundDetail> TtInboundDetails { get; set; } = null!;
        public virtual DbSet<ASNDetail> AsnDetails { get; set; } = null!;
        public virtual DbSet<TtPartMaster> TtPartMasters { get; set; } = null!;
        public virtual DbSet<TtExternalPid> TtExternalPids { get; set; } = null!;
        public virtual DbSet<TtLocation> TtLocations { get; set; } = null!;
        public virtual DbSet<TtStockTransfer> TtStockTransfers { get; set; } = null!;
        public virtual DbSet<TtStockTransferDetail> TtStockTransferDetails { get; set; } = null!;
        public virtual DbSet<TtStfReversalMaster> TtStfreversalMasters { get; set; } = null!;
        public virtual DbSet<TtStfReversalDetail> TtStfreversalDetails { get; set; } = null!;
        public virtual DbSet<TtInvAdjustmentDetail> TtInvAdjustmentDetails { get; set; } = null!;
        public virtual DbSet<TtInvAdjustmentMaster> TtInvAdjustmentMasters { get; set; } = null!;
        public virtual DbSet<TtInvTransaction> TtInvTransactions { get; set; } = null!;
        public virtual DbSet<TtInvTransactionPerSupplier> TtInvTransactionPerSuppliers { get; set; } = null!;
        public virtual DbSet<TtInvTransactionPerWh> TtInvTransactionPerWhs { get; set; } = null!;
        public virtual DbSet<TtQuarantineReason> TtQuarantineReasons { get; set; } = null!;
        public virtual DbSet<TtDecant> TtDecants { get; set; } = null!;
        public virtual DbSet<TtDecantDetail> TtDecantDetails { get; set; } = null!;
        public virtual DbSet<TtDecantPkg> TtDecantPkgs { get; set; } = null!;
        public virtual DbSet<TtCustomer> TtCustomers { get; set; } = null!;
        public virtual DbSet<TtOutbound> TtOutbounds { get; set; } = null!;
        public virtual DbSet<TtOutboundDetail> TtOutboundDetails { get; set; } = null!;
        public virtual DbSet<TtPidCode> TtPidCodes { get; set; } = null!;
        public virtual DbSet<TtUOM> TtUOM { get; set; } = null!;
        public virtual DbSet<TtUOMDecimal> TtUOMDecimals { get; set; } = null!;
        public virtual DbSet<SupplierMaster> SupplierMaster { get; set; } = null!;
        public virtual DbSet<SupplierDetail> SupplierDetails { get; set; } = null!;
        public virtual DbSet<ILogLocationCategory> ILogLocationCategories { get; set; } = null!;
        public virtual DbSet<TtRelocationLog> TtRelocationLog { get; set; } = null!;
        public virtual DbSet<TtPalletTransferRequest> TtPalletTransferRequests { get; set; } = null!;
        public virtual DbSet<EKanbanHeader> EKanbanHeaders { get; set; } = null!;
        public virtual DbSet<EKanbanDetail> EKanbanDetails { get; set; } = null!;
        public virtual DbSet<TtPickingList> TtPickingLists { get; set; } = null!;
        public virtual DbSet<TtPickingAllocatedPid> TtPickingAllocatedPids { get; set; } = null!;
        public virtual DbSet<TtPickingListEkanban> TtPickingListEkanban { get; set; } = null!;
        public virtual DbSet<ILogPickingRequest> ILogPickingRequests { get; set; } = null!;
        public virtual DbSet<ILogPickingRequestRevision> ILogPickingRequestRevisions { get; set; } = null!;
        public virtual DbSet<ILogPickingRequestRevisionItem> ILogPickingRequestRevisionItems { get; set; } = null!;
        public virtual DbSet<TtLoadingDetail> TtLoadingDetails { get; set; } = null!;
        public virtual DbSet<TtLoading> TtLoadings { get; set; } = null!;
        public virtual DbSet<TtUnloadingPoint> TtUnloadingPoints { get; set; } = null!;
        public virtual DbSet<TtPalletType> TtPalletTypes { get; set; } = null!;
        public virtual DbSet<TtInboundReversal> TtInboundReversals { get; set; } = null!;
        public virtual DbSet<TtInboundReversalDetail> TtInboundReversalDetails { get; set; } = null!;
        public virtual DbSet<BillingLog> BillingLogs { get; set; } = null!;
        public virtual DbSet<SupplierItemMaster> SupplierItems { get; set; } = null!;
        public virtual DbSet<InvoiceRequest> InvoiceRequests { get; set; } = null!;
        public virtual DbSet<InvoiceRequestProduct> InvoiceRequestProducts { get; set; } = null!;
        public virtual DbSet<InvoiceRequestBlocklist> InvoiceRequestBlocklist { get; set; } = null!;
        public virtual DbSet<InvoiceBatch> InvoiceBatches { get; set; } = null!;
        public virtual DbSet<InvoiceBatchRequestLink> InvoiceBatchRequestLinks { get; set; } = null!;
        public virtual DbSet<InvoicePriceValidation> InvoicePriceValidations { get; set; } = null!;
        public virtual DbSet<InvoicePriceValidationRequest> InvoicePriceValidationRequests { get; set; } = null!;
        public virtual DbSet<Invoice> Invoices { get; set; } = null!;
        public virtual DbSet<InvoiceBatchCustomsAgency> InvoiceBatchCustoms { get; set; } = null!;
        public virtual DbSet<InvoiceFile> InvoiceFiles { get; set; } = null!;
        public virtual DbSet<FactoryMaster> Factories { get; set; } = null!;
        public virtual DbSet<TtPriceMaster> Prices { get; set; } = null!;
        public virtual DbSet<TtAddressContact> AddressContacts { get; set; } = null!;
        public virtual DbSet<TtAddressBook> AddressBooks { get; set; } = null!;
        public virtual DbSet<TtCompanyProfile> CompanyProfiles { get; set; } = null!;
        public virtual DbSet<TtInventoryControl> InventoryControls { get; set; } = null!;
        public virtual DbSet<TtCustomerClient> CustomerClients { get; set; } = null!;
        public virtual DbSet<TtCountry> Countries { get; set; } = null!;
        public virtual DbSet<TtControlCode> ControlCodes { get; set; } = null!;
        public virtual DbSet<TtProductCode> ProductCodes { get; set; } = null!;
        public virtual DbSet<TtStockTakeByLoc> StockTakes { get; set; } = null!;
        public virtual DbSet<TtStockTakeByLocDetail> StockTakeDetails { get; set; } = null!;
        public virtual DbSet<TtStockTakeRelocationLog> StockTakeRelocationLogs { get; set; } = null!;
        public virtual DbSet<TtPackageType> PackageTypes { get; set; } = null!;
        public virtual DbSet<TtWarehouse> Warehouses { get; set; } = null!;
        public virtual DbSet<TtArea> Areas { get; set; } = null!;
        public virtual DbSet<TtAreaType> AreaTypes { get; set; } = null!;
        public virtual DbSet<TtLabelPrinter> LabelPrinters { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.HasDefaultSchema("dbo");
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);

        }
    }
}
