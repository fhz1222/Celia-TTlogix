using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;


namespace Persistence.Configurations;
public partial class AppDbContext
{
    class UOMDecimalConfigration : IEntityTypeConfiguration<TtUOMDecimal>
    {
        public void Configure(EntityTypeBuilder<TtUOMDecimal> entity)
        {
            entity.HasKey(e => new { e.CustomerCode, e.UOM });
            entity.ToTable("TT_UOMDecimal");
        }
    }

}
