using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class ExternalPidConfiguration : IEntityTypeConfiguration<TtExternalPid>
    {
        public void Configure(EntityTypeBuilder<TtExternalPid> entity)
        {
            entity.HasKey(e => e.Pid);

            entity.ToTable("TT_ExternalPID");

            entity.Property(e => e.Pid)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PID");

            entity.Property(e => e.ExternalPid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ExternalPID");

            entity.Property(e => e.InJobNo)
                .HasMaxLength(15)
                .IsUnicode(false);
        }
    }
}
