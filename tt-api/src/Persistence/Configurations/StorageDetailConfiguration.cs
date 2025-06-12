using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Entities;

public partial class AppDbContext
{
    class StorageDetailConfiguration : IEntityTypeConfiguration<TtStorageDetail>
    {
        public void Configure(EntityTypeBuilder<TtStorageDetail> entity)
        {
            entity.HasKey(e => e.Pid);

            entity.ToTable("TT_StorageDetail");

            entity.HasIndex(e => new { e.Status, e.ProductCode, e.Pid }, "INDEX1");

            entity.HasIndex(e => e.Status, "INDEX14");

            entity.HasIndex(e => new { e.ProductCode, e.CustomerCode, e.SupplierId }, "INDEX1_TT_StorageDetail");

            entity.HasIndex(e => new { e.SupplierId, e.CustomerCode, e.ProductCode, e.Whscode, e.LocationCode, e.Status, e.Qty }, "INDEX2");

            entity.HasIndex(e => new { e.Whscode, e.LocationCode, e.Status }, "INDEX3");

            entity.HasIndex(e => new { e.CustomerCode, e.Status }, "IN_TT_StorageDetail1");

            entity.HasIndex(e => new { e.CustomerCode, e.Status, e.SupplierId }, "IN_TT_StorageDetail11");

            entity.HasIndex(e => new { e.CustomerCode, e.Whscode, e.SupplierId, e.Ownership, e.Status }, "IN_TT_StorageDetail12");

            entity.HasIndex(e => new { e.InboundDate, e.Status }, "IN_TT_StorageDetail13");

            entity.HasIndex(e => e.InJobNo, "IN_TT_StorageDetail2");

            entity.HasIndex(e => new { e.CustomerCode, e.Whscode }, "IN_TT_StorageDetail3");

            entity.HasIndex(e => new { e.ProductCode, e.CustomerCode, e.Whscode }, "IN_TT_StorageDetail4");

            entity.HasIndex(e => new { e.InJobNo, e.Status }, "IN_TT_StorageDetail5");

            entity.HasIndex(e => e.OutJobNo, "IN_TT_StorageDetail6");

            entity.HasIndex(e => new { e.ProductCode, e.OutJobNo, e.SupplierId }, "IN_TT_StorageDetail7");

            entity.HasIndex(e => new { e.CustomerCode, e.Status }, "IN_TT_StorageDetail8");

            entity.HasIndex(e => e.GroupId, "TT_StorageDetail");

            entity.Property(e => e.Pid)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PID");

            entity.Property(e => e.AllocatedQty).HasColumnType("decimal(18, 6)");

            entity.Property(e => e.BuyingPrice)
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValueSql("((0))");

            entity.Property(e => e.ChargedDate).HasColumnType("datetime");

            entity.Property(e => e.ControlCode1)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.ControlCode2)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.ControlCode3)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.ControlCode4)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.ControlCode5)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.ControlCode6)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.ControlDate).HasColumnType("datetime");

            entity.Property(e => e.CustomerCode)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.DownloadBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.DownloadDate).HasColumnType("datetime");

            entity.Property(e => e.GrossWeight)
                .HasColumnType("numeric(18, 6)")
                .HasDefaultValueSql("((1))");

            entity.Property(e => e.GroupId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("GroupID");

            entity.Property(e => e.Height)
                .HasColumnType("numeric(18, 6)")
                .HasDefaultValueSql("((1))");

            entity.Property(e => e.InJobNo)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.InboundDate).HasColumnType("datetime");

            entity.Property(e => e.IsVmi).HasColumnName("IsVMI");

            entity.Property(e => e.Length)
                .HasColumnType("numeric(18, 6)")
                .HasDefaultValueSql("((1))");

            entity.Property(e => e.LocationCode)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.NetWeight)
                .HasColumnType("numeric(18, 6)")
                .HasDefaultValueSql("((1))");

            entity.Property(e => e.NoOfLabel).HasDefaultValueSql("((1))");

            entity.Property(e => e.OriginalQty).HasColumnType("decimal(18, 6)");

            entity.Property(e => e.OutJobNo)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.ParentId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ParentID");

            entity.Property(e => e.ProductCode)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.Property(e => e.PutawayBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.PutawayDate).HasColumnType("datetime");

            entity.Property(e => e.Qty).HasColumnType("decimal(18, 6)");

            entity.Property(e => e.QtyPerPkg)
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValueSql("((1))");

            entity.Property(e => e.SellingPrice)
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValueSql("((0))");

            entity.Property(e => e.SerialNo)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.SupplierId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SupplierID")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.Whscode)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("WHSCode");

            entity.Property(e => e.Width)
                .HasColumnType("numeric(18, 6)")
                .HasDefaultValueSql("((1))");
        }
    }
}
