using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;


namespace Persistence.Configurations;
public partial class AppDbContext
{
    class AddressBookConfigration : IEntityTypeConfiguration<TtAddressBook>
    {
        public void Configure(EntityTypeBuilder<TtAddressBook> entity)
        {
            entity.HasKey(e => e.Code);

            entity.ToTable("TT_AddressBook");

            entity.Property(e => e.Code)
                .HasMaxLength(12)
                .IsUnicode(false);

            entity.Property(e => e.Address1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.Address2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.Address3)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.Address4)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.CancelledBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.CancelledDate).HasColumnType("datetime");

            entity.Property(e => e.CompanyCode)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.Country)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.CreateBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.FaxNo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.PostCode)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

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
