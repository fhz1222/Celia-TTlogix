using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;


namespace Persistence.Configurations;
public partial class AppDbContext
{
    class CountryConfigration : IEntityTypeConfiguration<TtCountry>
    {
        public void Configure(EntityTypeBuilder<TtCountry> entity)
        {
            entity.HasKey(e => e.Code);

            entity.ToTable("TT_Country");

            entity.Property(e => e.Code)
                .HasMaxLength(2)
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
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.Status).HasDefaultValueSql("((1))");
        }
    }
}
