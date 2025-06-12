using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Entities
{
    public partial class AppDbContext
    {
        class InvAdjustmentMasterConfiguration : IEntityTypeConfiguration<TtInvAdjustmentMaster>
        {

            public void Configure(EntityTypeBuilder<TtInvAdjustmentMaster> entity)
            {
                entity.HasKey(e => e.JobNo);

                entity.ToTable("TT_InvAdjustmentMaster");

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

                entity.Property(e => e.JobType).HasComment("0= Normal ; 1=Undo ZeroOut");

                entity.Property(e => e.Reason)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RefNo)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.RevisedBy)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.RevisedDate).HasColumnType("datetime");

                entity.Property(e => e.Whscode)
                    .HasMaxLength(7)
                    .IsUnicode(false)
                    .HasColumnName("WHSCode")
                    .HasDefaultValueSql("('')");

            }
        }
    }
}
