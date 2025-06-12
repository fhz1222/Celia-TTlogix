using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class InvoiceBatchRequestLinkConfiguration : IEntityTypeConfiguration<InvoiceBatchRequestLink>
    {
        public void Configure(EntityTypeBuilder<InvoiceBatchRequestLink> entity)
        {
            entity.ToTable("InvoiceBatchRequestLink");

            entity.HasNoKey();

            entity.Property(e => e.InvoiceBatchId).HasColumnName("InvoiceBatchID");

            entity.Property(e => e.InvoiceRequestId).HasColumnName("InvoiceRequestID");
        }
    }
}