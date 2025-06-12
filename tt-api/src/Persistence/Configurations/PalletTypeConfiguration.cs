using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class PalletTypeConfiguration : IEntityTypeConfiguration<TtPalletType>
    {
        public void Configure(EntityTypeBuilder<TtPalletType> entity)
        {
            entity.ToTable("TT_PalletType");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
        }
    }
}
