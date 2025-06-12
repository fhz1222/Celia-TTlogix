using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class InvoicePriceValidationRequestConfiguration : IEntityTypeConfiguration<InvoicePriceValidationRequest>
    {
        public void Configure(EntityTypeBuilder<InvoicePriceValidationRequest> entity)
        {
            entity.HasNoKey();

            entity.ToTable("InvoicePriceValidationRequest");

            entity.Property(e => e.InvoicePriceValidationId).HasColumnName("InvoicePriceValidationID");

            entity.Property(e => e.InvoiceRequestId).HasColumnName("InvoiceRequestID");
        }
    }
}