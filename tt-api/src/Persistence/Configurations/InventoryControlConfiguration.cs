using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Entities
{
    public partial class AppDbContext
    {
        class InventoryControlConfiguration : IEntityTypeConfiguration<TtInventoryControl>
        {

            public void Configure(EntityTypeBuilder<TtInventoryControl> entity)
            {
                entity.HasKey(e => e.CustomerCode);

                entity.ToTable("TT_InventoryControl");

                entity.Property(e => e.CustomerCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Cc1type)
                    .HasMaxLength(7)
                    .IsUnicode(false)
                    .HasColumnName("CC1Type")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Cc2type)
                    .HasMaxLength(7)
                    .IsUnicode(false)
                    .HasColumnName("CC2Type")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Cc3type)
                    .HasMaxLength(7)
                    .IsUnicode(false)
                    .HasColumnName("CC3Type")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Cc4type)
                    .HasMaxLength(7)
                    .IsUnicode(false)
                    .HasColumnName("CC4Type")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Cc5type)
                    .HasMaxLength(7)
                    .IsUnicode(false)
                    .HasColumnName("CC5Type")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Cc6type)
                    .HasMaxLength(7)
                    .IsUnicode(false)
                    .HasColumnName("CC6Type")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Pc1type)
                    .HasMaxLength(7)
                    .IsUnicode(false)
                    .HasColumnName("PC1Type")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Pc2type)
                    .HasMaxLength(7)
                    .IsUnicode(false)
                    .HasColumnName("PC2Type")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Pc3type)
                    .HasMaxLength(7)
                    .IsUnicode(false)
                    .HasColumnName("PC3Type")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Pc4type)
                    .HasMaxLength(7)
                    .IsUnicode(false)
                    .HasColumnName("PC4Type")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.RevisedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.RevisedDate).HasColumnType("datetime");

                entity.Property(e => e.SelectControlCode)
                    .HasMaxLength(7)
                    .IsUnicode(false);
            }
        }
    }
}
