using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using TT.Common;
using TT.Core.Entities;

namespace TT.DB
{
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<AccessGroup> AccessGroups { get; set; }
        public DbSet<SystemModule> SystemModules { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Outbound> Outbounds { get; set; }
        public DbSet<OutboundDetail> OutboundDetails { get; set; }
        public DbSet<SupplierMaster> SupplierMasters { get; set; }
        public DbSet<EKanbanHeader> EKanbanHeaders { get; set; }
        public DbSet<EKanbanDetail> EKanbanDetails { get; set; }
        public DbSet<Loading> Loadings { get; set; }
        public DbSet<LoadingDetail> LoadingDetails { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<PickingList> PickingLists { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<PartMaster> PartMasters { get; set; }
        public DbSet<UOM> UOMs { get; set; }
        public DbSet<UOMDecimal> UOMDecimals { get; set; }
        public DbSet<ExternalPID> ExternalPIDs { get; set; }
        public DbSet<StorageDetail> StorageDetails { get; set; }
        public DbSet<StorageDetailGroup> StorageDetailGroups { get; set; }
        public DbSet<Inbound> Inbounds { get; set; }
        public DbSet<InboundDetail> InboundDetails { get; set; }
        public DbSet<PriceMaster> PriceMasters { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<JobCode> JobCodes { get; set; }
        public DbSet<EOrder> EOrders { get; set; }
        public DbSet<PickingListEKanban> PickingListEKanbans { get; set; }
        public DbSet<PickingAllocatedPID> PickingAllocatedPIDs { get; set; }
        public DbSet<ControlCode> ControlCodes { get; set; }
        public DbSet<InvTransaction> InvTransactions { get; set; }
        public DbSet<InvTransactionPerSupplier> InvTransactionsPerSupplier { get; set; }
        public DbSet<InvTransactionPerWHS> InvTransactionsPerWHS { get; set; }
        public DbSet<BillingLog> BillingLogs { get; set; }
        public DbSet<OutboundReleaseBondedLog> OutboundReleaseBondedLogs { get; set; }
        public DbSet<OutboundQRCode> OutboundQRCodes { get; set; }
        public DbSet<InventoryControl> InventoryControls { get; set; }
        public DbSet<ProductCode> ProductCodes { get; set; }
        public DbSet<PickingListFixLog> PickingListFixLogs { get; set; }
        public DbSet<AccessLock> AccessLocks { get; set; }
        public DbSet<PickingListAllocatedPID> PickingListAllocatedPIDs { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<PIDCode> PIDCodes { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<WHSTransfer> WHSTransfers { get; set; }
        public DbSet<WHSTransferDetail> WHSTransferDetails { get; set; }
        public DbSet<ShortfallCancelLog> ShortfallCancelLogs { get; set; }
        public DbSet<AccessRight> AccessRights { get; set; }
        public DbSet<ReportPrintingLog> ReportPrintingLogs { get; set; }
        public DbSet<ASNHeader> ASNHeaders { get; set; }
        public DbSet<ASNDetail> ASNDetails { get; set; }
        public DbSet<EPO> EPOs { get; set; }
        public DbSet<PackageType> PackageTypes { get; set; }
        public DbSet<CycleCount> CycleCounts { get; set; }
        public DbSet<CycleCountDetail> CycleCountDetails { get; set; }
        public DbSet<QuarantineLog> QuarantineLogs { get; set; }
        public DbSet<SupplierItemMaster> SupplierItemMasters { get; set; }
        public DbSet<FactoryMaster> FactoryMasters { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<ItemMaster> ItemMasters { get; set; }
        public DbSet<SunsetExpiredAlert> SunsetExpiredAlerts { get; set; }
        public DbSet<SupplierDetail> SupplierDetails { get; set; }
        public DbSet<LabelPrinter> LabelPrinters { get; set; }
        public DbSet<StockTransfer> StockTransfers { get; set; }
        public DbSet<StockTransferDetail> StockTransferDetails { get; set; }
        public DbSet<StockTransferUploadLog> StockTransferUploadLogs { get; set; }
        public DbSet<EStockTransferHeader> EStockTransferHeaders { get; set; }
        public DbSet<Core.Entities.ILogLocationCategory> ILogLocationCategories { get; set; }
        public DbSet<EStockTransferDetail> EStockTransferDetails { get; set; }
        public DbSet<DelforHeader> DelforHeaders { get; set; }
        public DbSet<DeljitHeader> DeljitHeaders { get; set; }
        public DbSet<UnloadingPoint> UnloadingPoint { get; set; }
        public DbSet<UnloadingPointDefault> UnloadingPointDefault { get; set; }
        public DbSet<PalletType> PalletType { get; set; }
        public DbSet<ELLISPalletType> ELLISPalletType { get; set; }

        private bool IsItaly { get; set; }

        public Context(DbContextOptions<Context> options, IOptions<AppSettings> appSettings) : base(options)
        {
            IsItaly = appSettings.Value.OwnerCode == OwnerCode.IT;
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            if (changeTrackingOff) { ChangeTracker.DetectChanges(); }
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (changeTrackingOff) { ChangeTracker.DetectChanges(); }
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasKey(c => new { c.Code });
            modelBuilder.Entity<Customer>().HasKey(c => new { c.Code, c.WHSCode });
            modelBuilder.Entity<Outbound>().HasKey(c => new { c.JobNo });
            modelBuilder.Entity<SupplierMaster>().HasKey(c => new { c.FactoryID, c.SupplierID });
            modelBuilder.Entity<OutboundDetail>().HasKey(c => new { c.JobNo, c.LineItem });
            modelBuilder.Entity<EKanbanHeader>().HasKey(c => new { c.OrderNo });
            modelBuilder.Entity<EKanbanDetail>().HasKey(c => new { c.OrderNo, c.ProductCode, c.SerialNo });
            modelBuilder.Entity<LoadingDetail>().HasKey(c => new { c.JobNo, c.OrderNo });
            modelBuilder.Entity<InboundDetail>().HasKey(c => new { c.JobNo, c.LineItem });
            modelBuilder.Entity<PickingList>().HasKey(c => new { c.JobNo, c.LineItem, c.SeqNo });
            modelBuilder.Entity<Inventory>().HasKey(c => new { c.CustomerCode, c.SupplierID, c.ProductCode1, c.WHSCode, c.Ownership });
            modelBuilder.Entity<PartMaster>().HasKey(c => new { c.CustomerCode, c.SupplierID, c.ProductCode1 });
            modelBuilder.Entity<UOMDecimal>().HasKey(c => new { c.CustomerCode, c.UOM });
            modelBuilder.Entity<PriceMaster>().HasKey(c => new { c.CustomerCode, c.SupplierID, c.ProductCode1 });
            modelBuilder.Entity<Location>().HasKey(c => new { c.Code, c.WHSCode });
            modelBuilder.Entity<EOrder>().HasKey(c => new { c.PartNo, c.PurchaseOrderNo, c.CardSerial });
            modelBuilder.Entity<PickingListEKanban>().HasKey(c => new { c.JobNo, c.LineItem, c.SeqNo });
            modelBuilder.Entity<PickingAllocatedPID>().HasKey(c => new { c.JobNo, c.LineItem, c.SerialNo });
            modelBuilder.Entity<InvTransaction>().HasKey(c => new { c.JobNo, c.ProductCode });
            modelBuilder.Entity<InvTransactionPerSupplier>().HasKey(c => new { c.JobNo, c.ProductCode, c.SupplierID, c.Ownership });
            modelBuilder.Entity<InvTransactionPerWHS>().HasKey(c => new { c.JobNo, c.ProductCode, c.WHSCode });
            modelBuilder.Entity<BillingLog>().HasKey(c => new { c.JobNo, c.FactoryID, c.SupplierID, c.ProductCode });
            modelBuilder.Entity<OutboundReleaseBondedLog>().HasKey(c => new { c.JobNo, c.PID });
            modelBuilder.Entity<PickingListFixLog>().HasKey(c => new { c.JobNo, c.PID });
            modelBuilder.Entity<PickingListAllocatedPID>().HasKey(c => new { c.JobNo, c.LineItem, c.SeqNo });
            modelBuilder.Entity<ErrorLog>().HasKey(c => new { c.JobNo, c.Method, c.ErrorMessage, c.CreatedDate });
            modelBuilder.Entity<WHSTransferDetail>().HasKey(c => new { c.JobNo, c.PID });
            modelBuilder.Entity<ShortfallCancelLog>().HasKey(c => new { c.JobNo, c.PID });
            modelBuilder.Entity<AccessRight>().HasKey(c => new { c.GroupCode, c.ModuleCode });
            modelBuilder.Entity<ASNDetail>().HasKey(c => new { c.ASNNo, c.LineItem });
            modelBuilder.Entity<EPO>().HasKey(c => new { c.PONo, c.POLineItem, c.SupplierID, c.FactoryID });
            modelBuilder.Entity<CycleCountDetail>().HasKey(c => new { c.JobNo, c.LineItem, c.SeqNo });
            modelBuilder.Entity<QuarantineLog>().HasKey(c => new { c.JobNo, c.LineItem });
            modelBuilder.Entity<SupplierItemMaster>().HasKey(c => new { c.FactoryID, c.SupplierID, c.ProductCode });
            modelBuilder.Entity<ItemMaster>().HasKey(c => new { c.FactoryID, c.SupplierID, c.ProductCode });
            modelBuilder.Entity<SunsetExpiredAlert>().HasKey(c => new { c.FactoryID, c.SupplierID, c.ProductCode });
            modelBuilder.Entity<SupplierDetail>().HasKey(c => new { c.SupplierID, c.FactoryID });
            modelBuilder.Entity<StockTransferDetail>().HasKey(c => new { c.JobNo, c.LineItem });
            modelBuilder.Entity<StockTransferUploadLog>().HasKey(c => new { c.UPLJobNo, c.LineItem });
            modelBuilder.Entity<EStockTransferDetail>().HasKey(c => new { c.OrderNo, c.ProductCode, c.SerialNo });
            modelBuilder.Entity<DelforHeader>().HasKey(c => new { c.EDIID });
            modelBuilder.Entity<DeljitHeader>().HasKey(c => new { c.EDIID });
            modelBuilder.Entity<UnloadingPoint>().HasKey(c => new { c.Id });
            modelBuilder.Entity<UnloadingPointDefault>().HasKey(c => new { c.Id });

            modelBuilder.Entity<Inbound>().Property(e => e.Status).HasConversion<byte>();
            modelBuilder.Entity<Inbound>().Property(e => e.TransType).HasConversion<byte>();
            modelBuilder.Entity<Loading>().Property(e => e.Status).HasConversion<byte>();
            modelBuilder.Entity<Outbound>().Property(e => e.TransType).HasConversion<byte>();
            modelBuilder.Entity<Outbound>().Property(e => e.Status).HasConversion<byte>();
            modelBuilder.Entity<StorageDetail>().Property(e => e.Ownership).HasConversion<byte>();
            modelBuilder.Entity<StorageDetail>().Property(e => e.Status).HasConversion<byte>();
            modelBuilder.Entity<InvTransactionPerSupplier>().Property(e => e.Ownership).HasConversion<byte>();
            modelBuilder.Entity<Inventory>().Property(e => e.Ownership).HasConversion<byte>();
            modelBuilder.Entity<CycleCount>().Property(e => e.Status).HasConversion<byte>();
            modelBuilder.Entity<Location>().Property(e => e.Type).HasConversion<byte>();
            modelBuilder.Entity<User>().Property(e => e.Status).HasConversion<byte>();
            modelBuilder.Entity<AccessGroup>().Property(e => e.Status).HasConversion<byte>();
            modelBuilder.Entity<StockTransfer>().Property(e => e.Status).HasConversion<byte>();
            modelBuilder.Entity<StockTransfer>().Property(e => e.TransferType).HasConversion<byte>();
            modelBuilder.Entity<EStockTransferHeader>().Property(e => e.Status).HasConversion<byte>();

            if (IsItaly) modelBuilder.Entity<StockTransfer>().Ignore("DESADV");
        }

        public void ChangeTrackingOff()
        {
            ChangeTracker.AutoDetectChangesEnabled = false;
            changeTrackingOff = true;
        }

        public void ChangeTrackingOn()
        {
            ChangeTracker.AutoDetectChangesEnabled = true;
            changeTrackingOff = false;
        }

        public static void InsertOrUpdateGraph(DbContext context, object rootEntity)
        {
            context.Update(rootEntity);
            context.SaveChanges();
        }

        private bool changeTrackingOff = false;
    }
}
