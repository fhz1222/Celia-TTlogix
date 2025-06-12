using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;
public partial class AppDbContext
{
    class StockTakeRelocationLogConfiguration : IEntityTypeConfiguration<TtStockTakeRelocationLog>
    {
        public void Configure(EntityTypeBuilder<TtStockTakeRelocationLog> entity)
        {
            entity.HasKey(e => new { e.JobNo, e.Pid });

            entity.ToTable("TT_StockTakeRelocationLog");

            entity.Property(e => e.JobNo)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.Pid)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PID");

            entity.Property(e => e.NewLocationCode)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.NewWhscode)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("NewWHSCode");

            entity.Property(e => e.OldLocationCode)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.OldWhscode)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("OldWHSCode");

            entity.Property(e => e.RelocatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.RelocatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.TransType).HasComment("0: EXTRA; 1:MISSING");
        }
    }
}
