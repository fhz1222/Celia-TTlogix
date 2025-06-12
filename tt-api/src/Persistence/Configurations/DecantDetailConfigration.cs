using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;


namespace Persistence.Configurations;
public partial class AppDbContext
{
    class DecantDetailConfigration : IEntityTypeConfiguration<TtDecantDetail>
    {
        public void Configure(EntityTypeBuilder<TtDecantDetail> entity)
        {
            entity.HasKey(e => new { e.JobNo, e.ParentId, e.SeqNo });

            entity.ToTable("TT_DecantDetail");

            entity.Property(e => e.JobNo)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.ParentId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ParentID");

            entity.Property(e => e.GrossWeight)
                .HasColumnType("numeric(18, 6)")
                .HasDefaultValueSql("((1))");

            entity.Property(e => e.Height)
                .HasColumnType("numeric(18, 6)")
                .HasDefaultValueSql("((1))");

            entity.Property(e => e.Length)
                .HasColumnType("numeric(18, 6)")
                .HasDefaultValueSql("((1))");

            entity.Property(e => e.NetWeight)
                .HasColumnType("numeric(18, 6)")
                .HasDefaultValueSql("((1))");

            entity.Property(e => e.Pid)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PID");

            entity.Property(e => e.ProductCode)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.Property(e => e.Qty).HasColumnType("decimal(18, 6)");

            entity.Property(e => e.SupplierId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SupplierID");

            entity.Property(e => e.Width)
                .HasColumnType("numeric(18, 6)")
                .HasDefaultValueSql("((1))");
        }
    }
}
