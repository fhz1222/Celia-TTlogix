using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Entities
{
    public partial class AppDbContext
    {
        internal class StfReversalMasterConfiguration: IEntityTypeConfiguration<TtStfReversalMaster>
        {

            public void Configure(EntityTypeBuilder<TtStfReversalMaster> entity)
            {
                entity.HasKey(e => e.JobNo);

                entity.ToTable("TT_STFReversalMaster");

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

                entity.Property(e => e.Reason)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RefNo)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.StfjobNo)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("STFJobNo");

                entity.Property(e => e.Whscode)
                    .HasMaxLength(7)
                    .IsUnicode(false)
                    .HasColumnName("WHSCode");

            }
        }
    }
}
