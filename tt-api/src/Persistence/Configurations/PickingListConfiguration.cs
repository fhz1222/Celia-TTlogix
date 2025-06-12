using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class PickingListConfiguration : IEntityTypeConfiguration<TtPickingList>
    {

        public void Configure(EntityTypeBuilder<TtPickingList> entity)
        {
            entity.HasKey(e => new { e.JobNo, e.LineItem, e.SeqNo });

            entity.ToTable("TT_PickingList");

            entity.HasIndex(e => e.Pid, "IN_TT_PickingList");

            entity.HasIndex(e => new { e.JobNo, e.PickedBy, e.PickedDate }, "_dta_index_TT_PickingList_14_616389265__K1_K21_K22");

            entity.HasIndex(e => new { e.JobNo, e.Qty }, "_dta_index_TT_PickingList_14_616389265__K1_K6");

            entity.Property(e => e.JobNo)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.AllocatedPid)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("AllocatedPID");

            entity.Property(e => e.ControlCode)
                .HasMaxLength(7)
                .IsUnicode(false);

            entity.Property(e => e.ControlCodeType).HasDefaultValueSql("((0))");

            entity.Property(e => e.ControlCodeValue)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.Property(e => e.ControlDate).HasColumnType("datetime");

            entity.Property(e => e.DownloadBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.DownloadDate).HasColumnType("datetime");

            entity.Property(e => e.DropPoint)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.InboundDate).HasColumnType("datetime");

            entity.Property(e => e.InboundJobNo)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.LocationCode)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.PackageId)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("PackageID");

            entity.Property(e => e.PackedBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.PackedDate).HasColumnType("datetime");

            entity.Property(e => e.PickedBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.PickedDate).HasColumnType("datetime");

            entity.Property(e => e.Pid)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PID");

            entity.Property(e => e.ProductCode)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.Property(e => e.ProductionLine)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.Qty).HasColumnType("numeric(18, 6)");

            entity.Property(e => e.SupplierId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SupplierID")
                .HasDefaultValueSql("('')");

            entity.Property(e => e.Whscode)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("WHSCode");
        }
    }
}
