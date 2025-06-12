using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;


namespace Persistence.Configurations;
public partial class AppDbContext
{
    class OutboundConfigration : IEntityTypeConfiguration<TtOutbound>
    {
        public void Configure(EntityTypeBuilder<TtOutbound> entity)
        {
            entity.HasKey(e => new { e.JobNo });

            entity.ToTable("TT_Outbound");

            entity.Property(e => e.JobNo)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.CustomerCode)
                .HasMaxLength(7)
                .IsUnicode(false);

            entity.Property(e => e.WhsCode)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("WHSCode");

            entity.Property(e => e.OSNo)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.RefNo)
               .HasMaxLength(30)
               .IsUnicode(false)
               .HasDefaultValueSql("('')");

            entity.Property(e => e.ETD).HasColumnType("datetime");

            entity.Property(e => e.Remark)
               .HasMaxLength(100)
               .IsUnicode(false)
               .HasDefaultValueSql("('')");

            entity.Property(e => e.RefNo)
               .HasMaxLength(30)
               .IsUnicode(false)
               .HasDefaultValueSql("('')");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");


            entity.Property(e => e.RevisedBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.RevisedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.CancelledBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.CancelledDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.DispatchedBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.DispatchedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.CommInvNo)
                .HasMaxLength(1292)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.DeliveryTo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.NewWHSCode)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.TransType).HasDefaultValueSql("((0))");
            entity.Property(e => e.Status).HasDefaultValueSql("((0))");
        }
    }

}
