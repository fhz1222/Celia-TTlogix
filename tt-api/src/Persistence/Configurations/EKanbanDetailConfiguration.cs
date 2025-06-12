using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class EKanbanDetailConfiguration : IEntityTypeConfiguration<EKanbanDetail>
    {
        public void Configure(EntityTypeBuilder<EKanbanDetail> entity)
        {
            entity.HasKey(e => new { e.OrderNo, e.ProductCode, e.SerialNo })
                   .HasName("PK_EHPASNDetail");

            entity.ToTable("EKANBANDetail");

            entity.HasIndex(e => e.OrderNo, "INDEX1");

            entity.HasIndex(e => e.QuantitySupplied, "INDEX2");

            entity.HasIndex(e => new { e.OrderNo, e.ProductCode, e.SupplierId, e.Quantity }, "_dta_index_EKANBANDetail_14_1190295300__K1_K2_K4_K6");

            entity.HasIndex(e => new { e.OrderNo, e.SupplierId, e.ProductCode, e.Quantity, e.QuantitySupplied, e.QuantityReceived }, "_dta_index_EKANBANDetail_14_1190295300__K1_K4_K2_K6_K7_K8");

            entity.Property(e => e.OrderNo)
                .HasMaxLength(25)
                .IsUnicode(false);

            entity.Property(e => e.ProductCode)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.Property(e => e.SerialNo)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.Property(e => e.BillingNo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.DropPoint)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");

            entity.Property(e => e.QuantityReceived).HasColumnType("decimal(18, 2)");

            entity.Property(e => e.QuantitySupplied).HasColumnType("decimal(18, 2)");

            entity.Property(e => e.SupplierId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SupplierID")
                .HasDefaultValueSql("('')");
        }
    }
}
