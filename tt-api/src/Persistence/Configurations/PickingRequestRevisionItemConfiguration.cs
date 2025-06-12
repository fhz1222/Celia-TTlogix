using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class PickingRequestRevisionItemConfiguration : IEntityTypeConfiguration<ILogPickingRequestRevisionItem>
    {
        public void Configure(EntityTypeBuilder<ILogPickingRequestRevisionItem> entity)
        {
            entity.HasKey(e => new { e.PickingRequestId, e.PickingRequestRevision, e.LineNo })
                .HasName("PK_PickingRequestRevisionItem");

            entity.ToTable("iLogPickingRequestRevisionItem", "dbi");

            entity.Property(e => e.PickingRequestId)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.Pid)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PID");

            entity.Property(e => e.ProductCode)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.Property(e => e.SupplierId)
                .HasMaxLength(10)
                .IsUnicode(false);
        }
    }
}
