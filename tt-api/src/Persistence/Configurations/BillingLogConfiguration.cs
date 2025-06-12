using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;
public partial class AppDbContext
{
    class BillingLogConfiguration : IEntityTypeConfiguration<BillingLog>
    {
        public void Configure(EntityTypeBuilder<BillingLog> entity)
        {
            entity.HasKey(e => new { e.JobNo, e.FactoryId, e.SupplierId, e.ProductCode });

            entity.ToTable("BillingLog");

            entity.Property(e => e.JobNo)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.FactoryId)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("FactoryID");

            entity.Property(e => e.SupplierId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SupplierID");

            entity.Property(e => e.ProductCode)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.Property(e => e.BillingNo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.CostCurrency)
                .HasMaxLength(5)
                .IsUnicode(false);

            entity.Property(e => e.CostPrice)
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValueSql("((0))");

            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 6)");

            entity.Property(e => e.RefNo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");
        }
    }
}
