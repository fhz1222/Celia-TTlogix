using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;
public partial class AppDbContext
{
    class PidCodeConfiguration : IEntityTypeConfiguration<TtPidCode>
    {
        public void Configure(EntityTypeBuilder<TtPidCode> entity)
        {
            entity.HasKey(e => e.PidNo);

            entity.ToTable("TT_PIDCode");

            entity.HasIndex(e => e.PidNo, "_dta_index_TT_PIDCode_14_132195521__K1");

            entity.Property(e => e.PidNo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PIDNo");

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");
        }
    }
}
