using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;


namespace Persistence.Configurations;
public partial class AppDbContext
{
    class DecantConfigration : IEntityTypeConfiguration<TtDecant>
    {
        public void Configure(EntityTypeBuilder<TtDecant> entity)
        {
            entity.HasKey(e => e.JobNo);

            entity.ToTable("TT_Decant");

            entity.Property(e => e.JobNo)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.CancelledBy)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.CancelledDate).HasColumnType("datetime");

            entity.Property(e => e.ConfirmedBy)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.ConfirmedDate).HasColumnType("datetime");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.CustomerCode)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.RefNo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.Remark)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.RevisedBy)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.RevisedDate).HasColumnType("datetime");

            entity.Property(e => e.Whscode)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("WHSCode");
        }
    }
}
