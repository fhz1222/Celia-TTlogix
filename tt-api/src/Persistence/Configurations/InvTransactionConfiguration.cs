using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Entities
{
    public partial class AppDbContext
    {
        class InvTransactionConfiguration : IEntityTypeConfiguration<TtInvTransaction>
        {

            public void Configure(EntityTypeBuilder<TtInvTransaction> entity)
            {
                entity.HasKey(e => new { e.JobNo, e.ProductCode });

                entity.ToTable("TT_InvTransaction");

                entity.HasIndex(e => e.SystemDateTime, "IN_INVTRANS_2");

                entity.HasIndex(e => new { e.ProductCode, e.CustomerCode, e.SystemDateTime, e.SystemDate, e.BalancePkg, e.BalanceQty, e.Pkg, e.Qty, e.Act, e.JobDate, e.JobNo }, "_dta_index_TT_InvTransaction_14_2023678257__K2_K3_K11_K10_K9_K8_K7_K6_K5_K4_K1");

                entity.Property(e => e.JobNo)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.ProductCode)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.JobDate).HasColumnType("datetime");

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
