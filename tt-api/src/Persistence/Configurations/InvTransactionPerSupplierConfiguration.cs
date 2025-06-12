using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Entities
{
    public partial class AppDbContext
    {
        class InvTransactionPerSupplierConfiguration : IEntityTypeConfiguration<TtInvTransactionPerSupplier>
        {

            public void Configure(EntityTypeBuilder<TtInvTransactionPerSupplier> entity)
            {
                entity.HasKey(e => new { e.JobNo, e.ProductCode, e.SupplierId, e.Ownership });

                entity.ToTable("TT_InvTransactionPerSupplier");

                entity.HasIndex(e => e.SystemDateTime, "IN_InvTransPerSupplier_1");

                entity.HasIndex(e => e.JobNo, "IN_InvTransPerSupplier_2");

                entity.HasIndex(e => new { e.ProductCode, e.SupplierId, e.CustomerCode }, "IN_InvTransPerSupplier_4");

                entity.Property(e => e.JobNo)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.ProductCode)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.SupplierId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SupplierID");

                entity.Property(e => e.BalanceQty).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.CustomerCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.JobDate).HasColumnType("datetime");

                entity.Property(e => e.Qty).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(CONVERT([datetime],CONVERT([char](10),getdate(),(103)),(103)))");

                entity.Property(e => e.SystemDateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            }
        }
    }
}
