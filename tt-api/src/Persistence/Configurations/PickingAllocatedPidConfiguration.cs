using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class PickingAllocatedPidConfiguration : IEntityTypeConfiguration<TtPickingAllocatedPid>
    {

        public void Configure(EntityTypeBuilder<TtPickingAllocatedPid> entity)
        {
            entity.HasKey(e => new { e.JobNo, e.LineItem, e.SerialNo })
                   .HasName("PK_TT_PickingAllocatedPID1");

            entity.ToTable("TT_PickingAllocatedPID");

            entity.Property(e => e.JobNo)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.AllocatedQty).HasColumnType("decimal(18, 6)");

            entity.Property(e => e.PickedQty).HasColumnType("decimal(18, 6)");

            entity.Property(e => e.Pid)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PID");
        }
    }
}
