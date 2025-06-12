using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class InboundDetailConfiguration : IEntityTypeConfiguration<TtInboundDetail>
    {
        public void Configure(EntityTypeBuilder<TtInboundDetail> entity)
        {
            entity.HasKey(e => new { e.JobNo, e.LineItem });

            entity.ToTable("TT_InboundDetail");

            entity.HasIndex(e => new { e.Asnno, e.AsnlineItem }, "IN_InboundDetail2");

            entity.HasIndex(e => new { e.JobNo, e.ProductCode, e.Qty, e.NoOfPackage }, "_dta_index_TT_InboundDetail_14_1879677744__K1_K4_K6_K7");

            entity.Property(e => e.JobNo)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.AsnlineItem).HasColumnName("ASNLineItem");

            entity.Property(e => e.Asnno)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ASNNo");

            entity.Property(e => e.BuyingPricePerLine)
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValueSql("((0))");

            entity.Property(e => e.ControlCode1)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.ControlCode2)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.ControlCode3)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.ControlCode4)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.ControlCode5)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.ControlCode6)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.Property(e => e.ControlDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.GrossWeight)
                .HasColumnType("numeric(18, 6)")
                .HasDefaultValueSql("((1))");

            entity.Property(e => e.Height)
                .HasColumnType("numeric(18, 6)")
                .HasDefaultValueSql("((1))");

            entity.Property(e => e.ImportedQty).HasColumnType("decimal(18, 6)");

            entity.Property(e => e.Length)
                .HasColumnType("numeric(18, 6)")
                .HasDefaultValueSql("((1))");

            entity.Property(e => e.NetWeight)
                .HasColumnType("numeric(18, 6)")
                .HasDefaultValueSql("((1))");

            entity.Property(e => e.NoOfLabel).HasDefaultValueSql("((1))");

            entity.Property(e => e.NoOfPackage).HasDefaultValueSql("((1))");

            entity.Property(e => e.PackageType)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.ProductCode)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.Property(e => e.Qty).HasColumnType("decimal(18, 6)");

            entity.Property(e => e.Remark)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.RevisedBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.RevisedDate).HasColumnType("datetime");

            entity.Property(e => e.Width)
                .HasColumnType("numeric(18, 6)")
                .HasDefaultValueSql("((1))");
        }
    }
}