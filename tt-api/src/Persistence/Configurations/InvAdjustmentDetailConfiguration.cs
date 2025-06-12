using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Entities
{
    public partial class AppDbContext
    {
        class InvAdjustmentDetailConfiguration : IEntityTypeConfiguration<TtInvAdjustmentDetail>
        {

            public void Configure(EntityTypeBuilder<TtInvAdjustmentDetail> entity)
            {
                entity.HasKey(e => new { e.JobNo, e.LineItem });

                entity.ToTable("TT_InvAdjustmentDetail");

                entity.Property(e => e.JobNo)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Pid)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PID");

                entity.Property(e => e.ProductCode)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Remark)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RevisedBy)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.RevisedDate).HasColumnType("datetime");
            }
        }
    }
}
