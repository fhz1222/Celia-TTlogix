using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;


namespace Persistence.Configurations;
public partial class AppDbContext
{
    class LocationConfigration : IEntityTypeConfiguration<TtLocation>
    {
        public void Configure(EntityTypeBuilder<TtLocation> entity)
        {
            entity.HasKey(e => new { e.Code, e.Whscode });

            entity.ToTable("TT_Location");

            entity.HasIndex(e => new { e.Whscode, e.Code, e.Type }, "_dta_index_TT_Location_14_2103678542__K2_K1_K7");

            entity.HasIndex(e => e.Type, "_dta_index_TT_Location_14_2103678542__K7");

            entity.Property(e => e.Code)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.Whscode)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("WHSCode");

            entity.Property(e => e.AreaCode)
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

            entity.Property(e => e.IsPriority).HasDefaultValueSql("((0))");

            entity.Property(e => e.M3).HasColumnType("numeric(18, 6)");

            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.ILogLocationCategoryId)
                .HasColumnName("ILogLocationCategoryId")
                .HasDefaultValueSql("((0))");

            entity.Property(e => e.Status).HasDefaultValueSql("((1))");

            entity.Property(e => e.ILogLocationCategoryId)
                .HasColumnName("ILogLocationCategoryId")
                .HasDefaultValueSql("((0))");
        }
    }
}
