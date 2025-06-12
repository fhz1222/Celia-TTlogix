using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class InvoiceBatchCustomsAgencyConfiguration : IEntityTypeConfiguration<InvoiceBatchCustomsAgency>
    {
        public void Configure(EntityTypeBuilder<InvoiceBatchCustomsAgency> entity)
        {
            entity.HasKey(e => e.InvoiceBatchId);

            entity.ToTable("InvoiceBatchCustomsAgency");

            entity.Property(e => e.InvoiceBatchId)
                .ValueGeneratedNever()
                .HasColumnName("InvoiceBatchID");

            entity.Property(e => e.Comment).HasMaxLength(256);
        }
    }
}