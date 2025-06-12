using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;


namespace Persistence.Configurations;
public partial class AppDbContext
{
    class SupplierMasterConfigration : IEntityTypeConfiguration<SupplierMaster>
    {
        public void Configure(EntityTypeBuilder<SupplierMaster> entity)
        {
            entity.HasKey(e => new { e.FactoryId, e.SupplierId });

            entity.ToTable("SupplierMaster");

            entity.Property(e => e.FactoryId)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("FactoryID");

            entity.Property(e => e.SupplierId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SupplierID");

            entity.Property(e => e.AgreementCode)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasDefaultValueSql("('V')");

            entity.Property(e => e.BlanketOrderNo)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.CompanyName).HasMaxLength(50);

            entity.Property(e => e.Country).HasMaxLength(40);

            entity.Property(e => e.NoEDIFlag)
                .HasColumnName("NoEDIFlag")
                .HasDefaultValueSql("((0))");

            entity.Property(e => e.PostCode).HasMaxLength(12);

            entity.Property(e => e.RevisedBy).HasMaxLength(12);

            entity.Property(e => e.RevisedDate).HasColumnType("datetime");

            entity.Property(e => e.SapVendorType).HasColumnName("SAPVendorType");

            entity.Property(e => e.SourceOfParts)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();

            entity.Property(e => e.Status).HasComment("0 = Inactive, 1 = Active");

            entity.Property(e => e.StreetAddress).HasMaxLength(50);

            entity.Property(e => e.Suburb).HasMaxLength(40);

            entity.Property(e => e.SupplyParadigm).HasMaxLength(2);
        }
    }
}
