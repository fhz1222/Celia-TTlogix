using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;
public partial class AppDbContext
{
    class StockTakeConfiguration : IEntityTypeConfiguration<TtStockTakeByLoc>
    {
        public void Configure(EntityTypeBuilder<TtStockTakeByLoc> entity)
        {
            entity.HasKey(e => e.JobNo)
                .HasName("PK_TT_StockTake");

            entity.ToTable("TT_StockTakeByLoc");

            entity.Property(e => e.JobNo)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.CancelledBy)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.CancelledDate).HasColumnType("datetime");

            entity.Property(e => e.CompletedBy)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.CompletedDate).HasColumnType("datetime");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.FixExtraBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.FixExtraDate).HasColumnType("datetime");

            entity.Property(e => e.FixMissingBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.FixMissingDate).HasColumnType("datetime");

            entity.Property(e => e.LocationCode)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.RefNo)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.Remark)
                .HasMaxLength(100)
                .IsUnicode(false);

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
