using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class ASNDetailConfiguration : IEntityTypeConfiguration<ASNDetail>
    {
        public void Configure(EntityTypeBuilder<ASNDetail> entity)
        {
            entity.HasKey(e => new { e.Asnno, e.LineItem });

            entity.ToTable("ASNDetail");

            entity.HasIndex(e => new { e.Asnno, e.ContainerNo, e.ProductCode }, "INDEX1");

            entity.HasIndex(e => new { e.ProductCode, e.OrderNo }, "IN_ASNDetail2");

            entity.HasIndex(e => e.Status, "IX_ASNDetail_Status_EFFD9");

            entity.HasIndex(e => new { e.InJobNo, e.Asnno }, "_dta_index_ASNDetail_14_1341963857__K13_K1");

            entity.Property(e => e.Asnno)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ASNNo");

            entity.Property(e => e.BatchNo).HasMaxLength(15);

            entity.Property(e => e.BillOfLading)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.ContainerNo)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.ContainerSize)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.ExSupplierDate).HasColumnType("datetime");

            entity.Property(e => e.InJobNo)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.IsSapschedulingAgreement).HasColumnName("IsSAPSchedulingAgreement");

            entity.Property(e => e.MaerskSono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MaerskSONo")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.ManufacturedDate).HasColumnType("datetime");

            entity.Property(e => e.OrderNo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.PolineNo)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("POLineNo")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.Pono)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("PONo")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.PortArrivalDate).HasColumnType("datetime");

            entity.Property(e => e.PreImportStatus)
                .HasMaxLength(3)
                .IsUnicode(false);

            entity.Property(e => e.ProductCode)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.Property(e => e.SealNo)
                .HasMaxLength(20)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.ShipDepartureDate).HasColumnType("datetime");

            entity.Property(e => e.Status)
                .HasMaxLength(3)
                .IsUnicode(false);

            entity.Property(e => e.StoreArrivalDate).HasColumnType("datetime");

            entity.Property(e => e.VesselName)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.Property(e => e.VoyageNo)
                .HasMaxLength(25)
                .IsUnicode(false);
        }
    }
}
