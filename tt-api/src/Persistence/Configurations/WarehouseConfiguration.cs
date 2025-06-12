using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;


namespace Persistence.Configurations;

public partial class AppDbContext
{
    class WarehouseConfigration : IEntityTypeConfiguration<TtWarehouse>
    {
        public void Configure(EntityTypeBuilder<TtWarehouse> entity)
        {
            entity.HasKey(e => e.Code);

            entity.ToTable("TT_Warehouse");

            entity.Property(e => e.Code)
                .HasMaxLength(7)
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

            entity.Property(e => e.Area).HasColumnType("numeric(18, 0)");

            entity.Property(e => e.CancelledBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.CancelledDate).HasColumnType("datetime");

            entity.Property(e => e.Country)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.FaxNo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.Pic)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PIC")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.PostCode)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.Status).HasDefaultValueSql("((1))");

            entity.Property(e => e.TelNo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");
        }
    }

}
