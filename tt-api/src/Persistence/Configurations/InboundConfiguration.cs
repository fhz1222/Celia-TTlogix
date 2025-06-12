using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Entities
{
    public partial class AppDbContext
    {
        class InboundConfiguration : IEntityTypeConfiguration<TtInbound>
        {

            public void Configure(EntityTypeBuilder<TtInbound> entity)
            {
                entity.HasKey(e => e.JobNo);

                entity.ToTable("TT_Inbound");

                entity.HasIndex(e => e.Irno, "IN_TT_Inbound_1");

                entity.HasIndex(e => e.Status, "idx_TT_Inbound_Status");

                entity.Property(e => e.JobNo)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.CancelledBy)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.CancelledDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Currency)
                    .HasMaxLength(7)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Eta)
                    .HasColumnType("datetime")
                    .HasColumnName("ETA")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Im4no)
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("IM4No");

                entity.Property(e => e.Irno)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("IRNo")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.PutawayBy)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.PutawayDate).HasColumnType("datetime");

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
}
