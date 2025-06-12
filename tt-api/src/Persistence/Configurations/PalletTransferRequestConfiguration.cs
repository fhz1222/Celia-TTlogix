using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Entities
{
    public partial class AppDbContext
    {
        class PalletTransferRequestConfiguration : IEntityTypeConfiguration<TtPalletTransferRequest>
        {
            public void Configure(EntityTypeBuilder<TtPalletTransferRequest> entity)
            {
                entity.HasKey(e => e.JobNo);

                entity.ToTable("TT_PalletTransferRequest");

                entity.Property(e => e.JobNo)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.CompletedOn).HasColumnType("datetime");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime");

                entity.Property(e => e.PID)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            }
        }
    }
}