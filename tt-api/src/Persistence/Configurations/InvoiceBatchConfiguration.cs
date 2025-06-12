using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class InvoiceBatchConfiguration : IEntityTypeConfiguration<InvoiceBatch>
    {
        public void Configure(EntityTypeBuilder<InvoiceBatch> entity)
        {
            entity.ToTable("InvoiceBatch");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.ApprovedBy).HasMaxLength(10);

            entity.Property(e => e.ApprovedDate).HasColumnType("datetime");

            entity.Property(e => e.BatchNumber)
                .HasMaxLength(256)
                .IsUnicode(false);

            entity.Property(e => e.CreatedBy).HasMaxLength(10);

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.FactoryId)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("FactoryID");

            entity.Property(e => e.RejectedBy).HasMaxLength(10);

            entity.Property(e => e.RejectedDate).HasColumnType("datetime");

            entity.Property(e => e.SupplierId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SupplierID");
        }
    }
}