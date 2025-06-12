using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Entities
{
    public partial class AppDbContext
    {
        class InvTransactionPerWhsConfiguration : IEntityTypeConfiguration<TtInvTransactionPerWh>
        {

            public void Configure(EntityTypeBuilder<TtInvTransactionPerWh> entity)
            {
                entity.HasKey(e => new { e.JobNo, e.ProductCode, e.Whscode });

                entity.ToTable("TT_InvTransactionPerWHS");

                entity.HasIndex(e => e.SystemDateTime, "IN_InvTransPerWHS_2");

                entity.HasIndex(e => new { e.Whscode, e.CustomerCode, e.ProductCode, e.SystemDateTime, e.SystemDate, e.BalancePkg, e.BalanceQty, e.Pkg, e.Qty, e.Act, e.JobDate, e.JobNo }, "_dta_index_TT_InvTransactionPerWHS_14_2039678314__K3_K4_K2_K12_K11_K10_K9_K8_K7_K6_K5_K1");

                entity.Property(e => e.JobNo)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.ProductCode)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Whscode)
                    .HasMaxLength(7)
                    .IsUnicode(false)
                    .HasColumnName("WHSCode");

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
