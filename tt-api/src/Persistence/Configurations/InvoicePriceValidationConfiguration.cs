using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class InvoicePriceValidationConfiguration : IEntityTypeConfiguration<InvoicePriceValidation>
    {
        public void Configure(EntityTypeBuilder<InvoicePriceValidation> entity)
        {
            entity.ToTable("InvoicePriceValidation");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.CreatedBy).HasMaxLength(10);

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.Currency)
                .HasMaxLength(7)
                .IsUnicode(false);

            entity.Property(e => e.InvoiceTotalValue).HasColumnType("decimal(18, 2)");

            entity.Property(e => e.TtlogixTotalValue)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("TTLogixTotalValue");
        }
    }
}