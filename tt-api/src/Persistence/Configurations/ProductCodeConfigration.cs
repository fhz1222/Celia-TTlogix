using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;


namespace Persistence.Configurations;
public partial class AppDbContext
{
    class ProductCodeConfigration : IEntityTypeConfiguration<TtProductCode>
    {
        public void Configure(EntityTypeBuilder<TtProductCode> entity)
        {
            entity.HasKey(e => e.Code);

            entity.ToTable("TT_ProductCode");

            entity.Property(e => e.Code)
                .HasMaxLength(7)
                .IsUnicode(false);

            entity.Property(e => e.CancelledBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.CancelledDate).HasColumnType("datetime");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.Property(e => e.Status).HasDefaultValueSql("((1))");
        }
    }
}
