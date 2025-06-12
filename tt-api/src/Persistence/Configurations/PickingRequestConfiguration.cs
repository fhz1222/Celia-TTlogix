using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class PickingRequestConfiguration : IEntityTypeConfiguration<ILogPickingRequest>
    {
        public void Configure(EntityTypeBuilder<ILogPickingRequest> entity)
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("iLogPickingRequest", "dbi");

            entity.Property(e => e.OutboundJobNo)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.Id)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasComputedColumnSql("concat('PRQ',right('0000000000'+CONVERT([nvarchar],[_Id],0),(10)))")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.Id).Metadata.SetBeforeSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);
        }
    }
}
