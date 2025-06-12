using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;
public partial class AppDbContext
{
    class LoadingDetailConfiguration : IEntityTypeConfiguration<TtLoadingDetail>
    {
        public void Configure(EntityTypeBuilder<TtLoadingDetail> entity)
        {
            entity.HasKey(e => new { e.JobNo, e.OrderNo })
                .HasName("PK_TT_OutboundLoadingDetail");

            entity.ToTable("TT_LoadingDetail");

            entity.HasIndex(e => e.OrderNo, "idxTT_LoadingDetail_OrderNo");

            entity.Property(e => e.JobNo)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.OrderNo)
                .HasMaxLength(25)
                .IsUnicode(false);

            entity.Property(e => e.AddedBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.AddedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.Etd)
                .HasColumnType("datetime")
                .HasColumnName("ETD");

            entity.Property(e => e.OutJobNo)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.SupplierId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SupplierID");
        }
    }

}
