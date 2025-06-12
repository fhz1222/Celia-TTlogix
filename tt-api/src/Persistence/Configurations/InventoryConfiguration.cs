using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Entities
{
    public partial class AppDbContext
    {
        class InventoryConfiguration : IEntityTypeConfiguration<TtInventory>
        {

            public void Configure(EntityTypeBuilder<TtInventory> entity)
            {
                entity.HasKey(e => new { e.CustomerCode, e.SupplierId, e.ProductCode1, e.Whscode, e.Ownership })
                    .HasName("PK_TT_InventoryDetail");

                entity.ToTable("TT_Inventory");

                entity.HasIndex(e => new { e.CustomerCode, e.Whscode }, "IN_TT_Inventory1");

                entity.HasIndex(e => new { e.ProductCode1, e.SupplierId, e.CustomerCode, e.Whscode, e.OnHandQty, e.OnHandPkg, e.AllocatedQty, e.AllocatedPkg, e.QuarantineQty, e.QuarantinePkg, e.TransitQty, e.TransitPkg, e.DiscrepancyQty }, "_dta_index_TT_Inventory_14_410484541__K3_K2_K1_K4_K5_K6_K7_K8_K9_K10_K11_K12_K13");

                entity.HasIndex(e => e.Ownership, "idx_TT_Inventory_Ownership");

                entity.Property(e => e.CustomerCode)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.SupplierId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SupplierID")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.ProductCode1)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Whscode)
                    .HasMaxLength(7)
                    .IsUnicode(false)
                    .HasColumnName("WHSCode");

                entity.Property(e => e.AllocatedPkg).HasDefaultValueSql("((0))");

                entity.Property(e => e.AllocatedQty).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.DiscrepancyQty).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.OnHandPkg).HasDefaultValueSql("((0))");

                entity.Property(e => e.OnHandQty).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.QuarantinePkg).HasDefaultValueSql("((0))");

                entity.Property(e => e.QuarantineQty).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.TransitPkg).HasDefaultValueSql("((0))");

                entity.Property(e => e.TransitQty)
                    .HasColumnType("decimal(18, 6)")
                    .HasDefaultValueSql("((0))");
            }
        }
    }
}
