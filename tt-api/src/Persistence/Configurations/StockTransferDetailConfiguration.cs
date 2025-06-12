using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;
public partial class AppDbContext
{
    class StockTransferDetailConfiguration : IEntityTypeConfiguration<TtStockTransferDetail>
    {
        public void Configure(EntityTypeBuilder<TtStockTransferDetail> entity)
        {
            entity.HasKey(e => new { e.JobNo, e.LineItem });

            entity.ToTable("TT_StockTransferDetail");

            entity.Property(e => e.JobNo)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.LocationCode)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.OriginalLocationCode)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.OriginalSupplierId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("OriginalSupplierID");

            entity.Property(e => e.OriginalWhscode)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("OriginalWHSCode");

            entity.Property(e => e.Pid)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PID")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.Qty).HasColumnType("decimal(18, 6)");

            entity.Property(e => e.TransferredBy)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.TransferredDate).HasColumnType("datetime");

            entity.Property(e => e.Whscode)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("WHSCode")
                .HasDefaultValueSql("('')");
        }
    }
}
