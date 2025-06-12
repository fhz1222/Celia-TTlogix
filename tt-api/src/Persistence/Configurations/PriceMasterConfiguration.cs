using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class PriceMasterConfiguration : IEntityTypeConfiguration<TtPriceMaster>
    {
        public void Configure(EntityTypeBuilder<TtPriceMaster> entity)
        {
            entity.HasKey(e => new { e.CustomerCode, e.SupplierId, e.ProductCode1 });

            entity.ToTable("TT_PriceMaster");

            entity.Property(e => e.CustomerCode)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.SupplierId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SupplierID");

            entity.Property(e => e.ProductCode1)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.Property(e => e.BuyingPrice).HasColumnType("decimal(18, 6)");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

            entity.Property(e => e.Currency)
                .HasMaxLength(7)
                .IsUnicode(false);

            entity.Property(e => e.LastUpdatedInbound)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.LastUpdatedOutbound)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.OutRevisedBy)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.OutRevisedDate).HasColumnType("datetime");

            entity.Property(e => e.RevisedBy)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.RevisedDate).HasColumnType("datetime");

            entity.Property(e => e.SellingPrice).HasColumnType("decimal(18, 6)");
        }
    }
}