using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class UnloadingPointConfiguration : IEntityTypeConfiguration<TtUnloadingPoint>
    {
        public void Configure(EntityTypeBuilder<TtUnloadingPoint> entity)
        {
            entity.ToTable("TT_UnloadingPoint");

            entity.Property(e => e.CustomerCode)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
        }
    }
}
