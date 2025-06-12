using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class PickingRequestRevisionConfiguration : IEntityTypeConfiguration<ILogPickingRequestRevision>
    {
        public void Configure(EntityTypeBuilder<ILogPickingRequestRevision> entity)
        {
            entity.HasKey(e => new { e.PickingRequestId, e.Revision })
                .HasName("PK_PickingRequestRevision");

            entity.ToTable("iLogPickingRequestRevision", "dbi");

            entity.Property(e => e.PickingRequestId)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.ClosedOn).HasColumnType("datetime");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
        }
    }
}
