using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;


namespace Persistence.Configurations;
public partial class AppDbContext
{
    class LabelPrinterConfigration : IEntityTypeConfiguration<TtLabelPrinter>
    {
        public void Configure(EntityTypeBuilder<TtLabelPrinter> entity)
        {
            entity.HasKey(e => e.Ip);

            entity.ToTable("TT_LabelPrinter");

            entity.Property(e => e.Ip)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("IP");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.RevisedBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.RevisedDate).HasColumnType("datetime");
        }
    }
}
