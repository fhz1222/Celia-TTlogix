using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;


namespace Persistence.Configurations;
public partial class AppDbContext
{
    class SupplierItemMasterConfigration : IEntityTypeConfiguration<SupplierItemMaster>
    {
        public void Configure(EntityTypeBuilder<SupplierItemMaster> entity)
        {
            entity.HasKey(e => new { e.FactoryId, e.SupplierId, e.ProductCode });

            entity.ToTable("SupplierItemMaster");

            entity.HasIndex(e => new { e.FactoryId, e.SupplierId }, "IN_SupplierItemMaster1");

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

            entity.Property(e => e.CurrentCost).HasColumnType("decimal(18, 6)");

            entity.Property(e => e.CurrentCostCurrency)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.CurrentCostEffectiveDate).HasColumnType("datetime");

            entity.Property(e => e.FutureCost).HasColumnType("decimal(18, 6)");

            entity.Property(e => e.FutureCostCurrency)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.FutureCostEffectiveDate).HasColumnType("datetime");

            entity.Property(e => e.PastCost).HasColumnType("decimal(18, 6)");

            entity.Property(e => e.PastCostCurrency)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.PastCostEffectiveDate).HasColumnType("datetime");
        }
    }
}
