using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;


namespace Persistence.Configurations;
public partial class AppDbContext
{
    class CustomerClientConfigration : IEntityTypeConfiguration<TtCustomerClient>
    {
        public void Configure(EntityTypeBuilder<TtCustomerClient> entity)
        {
            entity.HasKey(e => e.Code);

            entity.ToTable("TT_CustomerClient");

            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.BillingAddress)
                .HasMaxLength(12)
                .IsUnicode(false);

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
                .IsUnicode(false);

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.CustomerCode)
                .HasMaxLength(10)
                .IsUnicode(false);

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
