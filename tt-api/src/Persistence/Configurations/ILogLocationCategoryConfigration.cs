using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;


namespace Persistence.Configurations;
public partial class AppDbContext
{
    class ILogLocationCategoryConfigration : IEntityTypeConfiguration<ILogLocationCategory>
    {
        public void Configure(EntityTypeBuilder<ILogLocationCategory> entity)
        {
            entity.HasKey(x => x.Id);

            entity.ToTable("TT_ILogLocationCategory");
            
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Name");
        }
    }
}
