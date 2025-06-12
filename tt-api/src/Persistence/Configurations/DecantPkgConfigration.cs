using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;


namespace Persistence.Configurations;
public partial class AppDbContext
{
    class DecantPkgConfigration : IEntityTypeConfiguration<TtDecantPkg>
    {
        public void Configure(EntityTypeBuilder<TtDecantPkg> entity)
        {
            entity.HasKey(e => new { e.JobNo, e.Pid });

            entity.ToTable("TT_DecantPkg");

            entity.Property(e => e.JobNo)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.Pid)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PID");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

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
