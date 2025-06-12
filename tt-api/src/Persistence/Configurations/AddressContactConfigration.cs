using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;


namespace Persistence.Configurations;
public partial class AppDbContext
{
    class AddressContactConfigration : IEntityTypeConfiguration<TtAddressContact>
    {
        public void Configure(EntityTypeBuilder<TtAddressContact> entity)
        {
            entity.HasKey(e => e.Code);

            entity.ToTable("TT_AddressContact");

            entity.Property(e => e.Code)
                .HasMaxLength(7)
                .IsUnicode(false);

            entity.Property(e => e.AddressCode)
                .HasMaxLength(12)
                .IsUnicode(false);

            entity.Property(e => e.CancelledBy)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.CancelledDate).HasColumnType("datetime");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.FaxNo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.Property(e => e.RevisedBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.RevisedDate).HasColumnType("datetime");

            entity.Property(e => e.Status).HasDefaultValueSql("((1))");

            entity.Property(e => e.TelNo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");
        }
    }
}
