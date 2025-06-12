using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;
public partial class AppDbContext
{
    class StockTakeDetailConfiguration : IEntityTypeConfiguration<TtStockTakeByLocDetail>
    {
        public void Configure(EntityTypeBuilder<TtStockTakeByLocDetail> entity)
        {
            entity.HasKey(e => new { e.JobNo, e.Pid });

            entity.ToTable("TT_StockTakeByLocDetail");

            entity.Property(e => e.JobNo)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.Pid)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PID");

            entity.Property(e => e.LocationCode)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.UoloadedDate).HasColumnType("datetime");

            entity.Property(e => e.UploadedBy)
                .HasMaxLength(10)
                .IsUnicode(false);
        }
    }
}
