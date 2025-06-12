using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class LoadingConfiguration : IEntityTypeConfiguration<TtLoading>
    {
        public void Configure(EntityTypeBuilder<TtLoading> entity)
        {
            entity.HasKey(e => e.JobNo);

            entity.ToTable("TT_Loading");

            entity.Property(e => e.JobNo)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.AllowedForDispatchModifiedBy)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.AllowedForDispatchModifiedDate).HasColumnType("datetime");

            entity.Property(e => e.CancelledBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.CancelledDate).HasColumnType("datetime");

            entity.Property(e => e.ConfirmedBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.ConfirmedDate).HasColumnType("datetime");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.CustomerCode)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.DockNo)
                .HasMaxLength(3)
                .IsUnicode(false);

            entity.Property(e => e.Eta)
                .HasColumnType("datetime")
                .HasColumnName("ETA");

            entity.Property(e => e.Etd)
                .HasColumnType("datetime")
                .HasColumnName("ETD");

            entity.Property(e => e.NoOfPallet).HasDefaultValueSql("((0))");

            entity.Property(e => e.RefNo)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.Remark)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.RevisedBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.RevisedDate).HasColumnType("datetime");

            entity.Property(e => e.TrailerNo)
                .HasMaxLength(8)
                .IsUnicode(false);

            entity.Property(e => e.TruckArrivalDate).HasColumnType("datetime");

            entity.Property(e => e.TruckDepartureDate).HasColumnType("datetime");

            entity.Property(e => e.TruckLicencePlate)
                .HasMaxLength(99)
                .IsUnicode(false);

            entity.Property(e => e.TruckSeqNo)
                .HasMaxLength(99)
                .IsUnicode(false);

            entity.Property(e => e.Whscode)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("WHSCode");
        }
    }
}