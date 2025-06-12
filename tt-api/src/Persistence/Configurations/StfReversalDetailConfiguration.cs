using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Entities
{
    public partial class AppDbContext
    {
        internal class StfReversalDetailConfiguration : IEntityTypeConfiguration<TtStfReversalDetail>
        {

            public void Configure(EntityTypeBuilder<TtStfReversalDetail> entity)
            {
                entity.HasKey(e => new { e.JobNo, e.Pid });

                entity.ToTable("TT_STFReversalDetail");

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

                entity.Property(e => e.OriginalLocationCode)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.OriginalSupplierId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("OriginalSupplierID");

                entity.Property(e => e.OriginalWhscode)
                    .HasMaxLength(7)
                    .IsUnicode(false)
                    .HasColumnName("OriginalWHSCode");

                entity.Property(e => e.ProductCode)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.TransferredBy)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.TransferredDate).HasColumnType("datetime");

                entity.Property(e => e.Whscode)
                    .HasMaxLength(7)
                    .IsUnicode(false)
                    .HasColumnName("WHScode");
            }
        }
    }
}
