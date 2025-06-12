using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class InvoiceRequestBlocklistConfiguration : IEntityTypeConfiguration<InvoiceRequestBlocklist>
    {
        public void Configure(EntityTypeBuilder<InvoiceRequestBlocklist> entity)
        {
            entity.HasKey(e => e.JobNo);

            entity.ToTable("InvoiceRequestBlocklist");

            entity.Property(e => e.JobNo)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.CreatedBy).HasMaxLength(10);

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");
        }
    }
}
