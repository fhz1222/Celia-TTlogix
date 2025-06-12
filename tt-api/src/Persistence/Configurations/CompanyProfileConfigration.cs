using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;


namespace Persistence.Configurations;
public partial class AppDbContext
{
    class CompanyProfileConfigration : IEntityTypeConfiguration<TtCompanyProfile>
    {
        public void Configure(EntityTypeBuilder<TtCompanyProfile> entity)
        {
            entity.HasKey(e => e.Code);

            entity.ToTable("TT_CompanyProfile");

            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.CancelledBy)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.CancelledDate).HasColumnType("datetime");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.RevisedBy)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.RevisedDate).HasColumnType("datetime");
        }
    }
}
