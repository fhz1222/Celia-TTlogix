using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;


namespace Persistence.Configurations;
public partial class AppDbContext
{
    class CustomerConfigration : IEntityTypeConfiguration<TtCustomer>
    {
        public void Configure(EntityTypeBuilder<TtCustomer> entity)
        {
            entity.HasKey(e => new { e.Code, e.Whscode });

            entity.ToTable("TT_Customer");

            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.Whscode)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("WHSCode");

            entity.Property(e => e.BillingAddress)
                .HasMaxLength(12)
                .IsUnicode(false);

            entity.Property(e => e.BizUnit)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.Buname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BUName")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.CancelledBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.CancelledDate).HasColumnType("datetime");

            entity.Property(e => e.CompanyCode)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.Pic1)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("PIC1");

            entity.Property(e => e.Pic2)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("PIC2")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.PrimaryAddress)
                .HasMaxLength(12)
                .IsUnicode(false);

            entity.Property(e => e.RevisedBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.RevisedDate).HasColumnType("datetime");

            entity.Property(e => e.ShippingAddress)
                .HasMaxLength(12)
                .IsUnicode(false);

            entity.Property(e => e.Status).HasDefaultValueSql("((1))");
        }
    }
}
