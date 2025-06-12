using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class PickingListEkanbanConfiguration : IEntityTypeConfiguration<TtPickingListEkanban>
    {
        public void Configure(EntityTypeBuilder<TtPickingListEkanban> entity)
        {
            entity.HasKey(e => new { e.JobNo, e.LineItem, e.SeqNo });

            entity.ToTable("TT_PickingListEKanban");

            entity.Property(e => e.JobNo)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.OrderNo)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.ProductCode)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.SerialNo)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");
        }
    }
}
