using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Entities
{
    public partial class AppDbContext
    {
        class PartMasterConfiguration : IEntityTypeConfiguration<TtPartMaster>
        {

            public void Configure(EntityTypeBuilder<TtPartMaster> entity)
            {
                entity.HasKey(e => new { e.CustomerCode, e.SupplierId, e.ProductCode1 });

                entity.ToTable("TT_PartMaster");

                entity.Property(e => e.CustomerCode)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.SupplierId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SupplierID");

                entity.Property(e => e.ProductCode1)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.BoxesInPallet).HasDefaultValueSql("((1))");

                entity.Property(e => e.CpartSpq)
                    .HasColumnType("decimal(18, 6)")
                    .HasColumnName("CPartSPQ");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.DoNotSyncEdi).HasColumnName("DoNotSyncEDI");

                entity.Property(e => e.EnableSerialNo).HasDefaultValueSql("((1))");

                entity.Property(e => e.FloorStackability).HasDefaultValueSql("((1))");

                entity.Property(e => e.GrossWeight)
                    .HasColumnType("numeric(18, 6)")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.GrossWeightTt)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("GrossWeightTT")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Height)
                    .HasColumnType("numeric(18, 6)")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.HeightTt)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("HeightTT")
                    .HasDefaultValueSql("((0.1))");

                entity.Property(e => e.IsCpart).HasColumnName("IsCPart");

                entity.Property(e => e.IsStandardPackaging).HasDefaultValueSql("((1))");

                entity.Property(e => e.Length)
                    .HasColumnType("numeric(18, 6)")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.LengthTt)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("LengthTT")
                    .HasDefaultValueSql("((0.1))");

                entity.Property(e => e.MasterSlave)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.NetWeight)
                    .HasColumnType("numeric(18, 6)")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.NetWeightTt)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("NetWeightTT")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.OrderLot)
                    .HasColumnType("decimal(18, 6)")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.OriginCountry)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.PackageType)
                    .HasMaxLength(7)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.ProductCode2)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.ProductCode3)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.ProductCode4)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.RevisedBy)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.RevisedDate).HasColumnType("datetime");

                entity.Property(e => e.Spq)
                    .HasColumnType("decimal(18, 6)")
                    .HasColumnName("SPQ")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");

                entity.Property(e => e.TruckStackability).HasDefaultValueSql("((1))");

                entity.Property(e => e.Uom)
                    .HasMaxLength(7)
                    .IsUnicode(false)
                    .HasColumnName("UOM")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Width)
                    .HasColumnType("numeric(18, 6)")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.WidthTt)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("WidthTT")
                    .HasDefaultValueSql("((0.1))");
            }
        }
    }
}
