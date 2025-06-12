using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> entity)
        {
            entity.ToTable("Invoice");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.Currency).HasMaxLength(7);

            entity.Property(e => e.InvoiceBatchId).HasColumnName("InvoiceBatchID");

            entity.Property(e => e.InvoiceNumber).HasMaxLength(256);

            entity.Property(e => e.Value).HasColumnType("decimal(18, 2)");
        }
    }
}