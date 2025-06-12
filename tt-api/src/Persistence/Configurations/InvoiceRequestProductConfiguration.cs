using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class InvoiceRequestProductConfiguration : IEntityTypeConfiguration<InvoiceRequestProduct>
    {
        public void Configure(EntityTypeBuilder<InvoiceRequestProduct> entity)
        {
            entity.ToTable("InvoiceRequestProduct");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.AsnNo)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ASNNo");

            entity.Property(e => e.InboundJob)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.PoLineNo)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("POLineNo");

            entity.Property(e => e.PoNumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("PONumber");

            entity.Property(e => e.ProductCode)
                .HasMaxLength(30)
                .IsUnicode(false);
        }
    }
}