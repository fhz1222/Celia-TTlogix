using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;


namespace Persistence.Configurations;
public partial class AppDbContext
{
    class RelocationLogConfiguration : IEntityTypeConfiguration<TtRelocationLog>
    {
        public void Configure(EntityTypeBuilder<TtRelocationLog> entity)
        {
            entity.HasKey(e => new { e.PID, e.RelocatedDate });

            entity.ToTable("TT_RelocationLog");

            entity.Property(e => e.PID)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.ExternalPID)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.OldWHSCode)
                .HasMaxLength(7)
                .IsUnicode(false);

            entity.Property(e => e.OldLocationCode)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.NewWHSCode)
                .HasMaxLength(7)
                .IsUnicode(false);

            entity.Property(e => e.NewLocationCode)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.ScannerType);

            entity.Property(e => e.RelocatedBy)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.RelocatedDate)
                .HasColumnType("datetime");
        }
    }
}
