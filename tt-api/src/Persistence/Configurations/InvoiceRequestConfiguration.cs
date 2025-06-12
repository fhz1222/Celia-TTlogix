using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class InvoiceRequestConfiguration : IEntityTypeConfiguration<InvoiceRequest>
    {
        public void Configure(EntityTypeBuilder<InvoiceRequest> entity)
        {
            entity.ToTable("InvoiceRequest");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.ApprovedBatchId).HasColumnName("ApprovedBatchID");

            entity.Property(e => e.CreatedBy).HasMaxLength(10);

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.FactoryId)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("FactoryID");

            entity.Property(e => e.JobNo)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.SupplierId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SupplierID");

            entity.Property(e => e.SupplierRefNo)
                .HasMaxLength(30)
                .IsUnicode(false);
        }
    }
}