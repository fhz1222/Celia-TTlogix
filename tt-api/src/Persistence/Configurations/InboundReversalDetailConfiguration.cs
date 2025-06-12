using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class InboundReversalDetailConfiguration : IEntityTypeConfiguration<TtInboundReversalDetail>
    {
        public void Configure(EntityTypeBuilder<TtInboundReversalDetail> entity)
        {
            entity.HasKey(e => new { e.JobNo, e.Pid });

            entity.ToTable("TT_InboundReversalDetail");

            entity.Property(e => e.JobNo)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.Pid)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PID");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.ProductCode)
                .HasMaxLength(30)
                .IsUnicode(false);
        }
    }
}
