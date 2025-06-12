using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;


namespace Persistence.Configurations;

public partial class AppDbContext
{
    class AreaConfigration : IEntityTypeConfiguration<TtArea>
    {
        public void Configure(EntityTypeBuilder<TtArea> entity)
        {
            entity.HasKey(e => new { e.Code, e.WhsCode });

            entity.ToTable("TT_Area");

            entity.Property(e => e.Code)
                .HasMaxLength(7)
                .IsUnicode(false);

            entity.Property(e => e.WhsCode)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("WHSCode");

            entity.Property(e => e.CancelledBy)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.CancelledDate).HasColumnType("datetime");

            entity.Property(e => e.Capacity).HasColumnType("numeric(18, 0)");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.Status).HasDefaultValueSql("((1))");

            entity.Property(e => e.Type)
                .HasMaxLength(7)
                .IsUnicode(false);
        }
    }

}
