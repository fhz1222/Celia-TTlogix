using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;
public partial class AppDbContext
{
    class StockTransferConfiguration : IEntityTypeConfiguration<TtStockTransfer>
    {
        public void Configure(EntityTypeBuilder<TtStockTransfer> entity)
        {
            entity.HasKey(e => e.JobNo);

            entity.ToTable("TT_StockTransfer");

            entity.Property(e => e.JobNo)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.CancelledBy)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.CancelledDate).HasColumnType("datetime");

            entity.Property(e => e.CommInvDate).HasColumnType("date");

            entity.Property(e => e.CommInvNo)
                .HasMaxLength(1292)
                .IsUnicode(false);

            entity.Property(e => e.ConfirmedBy)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.ConfirmedDate).HasColumnType("datetime");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.CustomerCode)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.Desadv)
                .HasColumnName("DESADV")
                .HasDefaultValueSql("((0))");

            entity.Property(e => e.RefNo)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.Remark)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.RevisedBy)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.RevisedDate).HasColumnType("datetime");

            entity.Property(e => e.WhsCode)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("WHSCode")
                .HasDefaultValueSql("('')");
        }
    }
}
