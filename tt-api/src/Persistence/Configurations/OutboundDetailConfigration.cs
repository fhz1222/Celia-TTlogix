using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class OutboundDetailConfigration : IEntityTypeConfiguration<TtOutboundDetail>
    {
        public void Configure(EntityTypeBuilder<TtOutboundDetail> entity)
        {
            entity.HasKey(e => new { e.JobNo, e.LineItem });

            entity.ToTable("TT_OutboundDetail");

            entity.Property(e => e.JobNo)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.BillingReportNo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.CostCurrency)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.CostPrice)
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValueSql("((0))");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.PickedQty).HasColumnType("decimal(18, 6)");

            entity.Property(e => e.ProductCode)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.Property(e => e.Qty).HasColumnType("decimal(18, 6)");

            entity.Property(e => e.RevisedBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.RevisedDate).HasColumnType("datetime");

            entity.Property(e => e.Status).HasDefaultValueSql("((0))");

            entity.Property(e => e.SupplierID)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");
        }
    }
}
