using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class FactoryMasterConfiguration : IEntityTypeConfiguration<FactoryMaster>
    {
        public void Configure(EntityTypeBuilder<FactoryMaster> entity)
        {
            entity.HasKey(e => e.FactoryId);

            entity.ToTable("FactoryMaster");

            entity.Property(e => e.FactoryId)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("FactoryID");

            entity.Property(e => e.FactoryName)
                .HasMaxLength(25)
                .IsUnicode(false);

            entity.Property(e => e.HasSap).HasColumnName("HasSAP");
        }
    }
}
