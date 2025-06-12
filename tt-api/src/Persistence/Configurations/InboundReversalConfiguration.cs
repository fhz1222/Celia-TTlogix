using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class InboundReversalConfiguration : IEntityTypeConfiguration<TtInboundReversal>
    {
        public void Configure(EntityTypeBuilder<TtInboundReversal> entity)
        {
            entity.HasKey(e => e.JobNo);

            entity.ToTable("TT_InboundReversalMaster");

            entity.Property(e => e.JobNo)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.CancelledBy)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.CancelledDate).HasColumnType("datetime");

            entity.Property(e => e.ConfirmedBy)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.ConfirmedDate).HasColumnType("datetime");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.CustomerCode)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.InJobNo)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.Reason)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.RefNo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.SupplierId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SupplierID");

            entity.Property(e => e.WhsCode)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("WHSCode");
        }
    }
}
