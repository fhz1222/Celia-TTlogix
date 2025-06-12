using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;
public partial class AppDbContext
{
    class QuarantineReasonConfiguration : IEntityTypeConfiguration<TtQuarantineReason>
    {
        public void Configure(EntityTypeBuilder<TtQuarantineReason> entity)
        {
            entity.HasKey(e => e.Pid);

            entity.ToTable("TT_QuarantineReason");

            entity.Property(e => e.Pid)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PID");

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.Reason)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

        }
    }
}
