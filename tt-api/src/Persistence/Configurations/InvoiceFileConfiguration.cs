using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class InvoiceFileConfiguration : IEntityTypeConfiguration<InvoiceFile>
    {
        public void Configure(EntityTypeBuilder<InvoiceFile> entity)
        {
            entity.ToTable("InvoiceFile");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.FileName).HasMaxLength(256);

            entity.Property(e => e.InvoiceId).HasColumnName("InvoiceID");
        }
    }
}