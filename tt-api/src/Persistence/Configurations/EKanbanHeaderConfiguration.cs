using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class EKanbanHeaderConfiguration : IEntityTypeConfiguration<EKanbanHeader>
    {
        public void Configure(EntityTypeBuilder<EKanbanHeader> entity)
        {
            entity.HasKey(e => e.OrderNo)
                .HasName("PK_EKanbanHeader_1");

            entity.ToTable("EKANBANHeader");

            entity.HasIndex(e => new { e.FactoryId, e.OrderNo, e.Status, e.Eta }, "INDEX1");

            entity.HasIndex(e => e.Status, "IN_EKANBANHeader_Status");

            entity.HasIndex(e => new { e.FactoryId, e.CreatedDate, e.OrderNo, e.RunNo, e.OutJobNo }, "_dta_index_EKANBANHeader_14_1608392799__K2_K5_K1_K3_K9");

            entity.Property(e => e.OrderNo)
                .HasMaxLength(25)
                .IsUnicode(false);

            entity.Property(e => e.BlanketOrderNo)
                .HasMaxLength(35)
                .IsUnicode(false);

            entity.Property(e => e.ConfirmationDate).HasColumnType("datetime");

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.Eta)
                .HasColumnType("datetime")
                .HasColumnName("ETA");

            entity.Property(e => e.FactoryId)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("FactoryID");

            entity.Property(e => e.Instructions)
                .HasMaxLength(120)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.IssuedDate).HasColumnType("datetime");

            entity.Property(e => e.OutJobNo)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.RefNo)
                .HasMaxLength(25)
                .IsUnicode(false);

            entity.Property(e => e.RunNo)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.Status).HasComment("0=New, 1=Imported, 2=InTransit, 3=Data sent, 4=Completed");
        }
    }
}
