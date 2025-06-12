using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Configurations;

public partial class AppDbContext
{
    class SupplierDetailConfigration : IEntityTypeConfiguration<SupplierDetail>
    {
        public void Configure(EntityTypeBuilder<SupplierDetail> entity)
        {
            entity.HasKey(e => new { e.SupplierId, e.FactoryId });

            entity.ToTable("SupplierDetail");

            entity.HasIndex(e => e.InvoiceFrequency, "_dta_index_SupplierDetail_14_424388581__K7");

            entity.Property(e => e.SupplierId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SupplierID");

            entity.Property(e => e.FactoryId)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("FactoryID");

            entity.Property(e => e.AgreementCode)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();

            entity.Property(e => e.CommitWip).HasColumnName("CommitWIP");

            entity.Property(e => e.DelFreqA)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();

            entity.Property(e => e.DelFreqB)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();

            entity.Property(e => e.DelFreqC)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();

            entity.Property(e => e.DelFreqX)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();

            entity.Property(e => e.EmergencyContact).HasMaxLength(50);

            entity.Property(e => e.FrequencyText)
                .HasMaxLength(95)
                .IsUnicode(false);

            entity.Property(e => e.InvoiceBatchSequenceNo).HasDefaultValueSql("((1))");

            entity.Property(e => e.InvoiceFrequency)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();

            entity.Property(e => e.LateAsntolerance).HasColumnName("LateASNTolerance");

            entity.Property(e => e.LeadTimeText)
                .HasMaxLength(90)
                .IsUnicode(false);

            entity.Property(e => e.Llead1A).HasColumnName("LLead1A");

            entity.Property(e => e.Llead1B).HasColumnName("LLead1B");

            entity.Property(e => e.Llead1C).HasColumnName("LLead1C");

            entity.Property(e => e.Llead1X).HasColumnName("LLead1X");

            entity.Property(e => e.Llead2A).HasColumnName("LLead2A");

            entity.Property(e => e.Llead2B).HasColumnName("LLead2B");

            entity.Property(e => e.Llead2C).HasColumnName("LLead2C");

            entity.Property(e => e.Llead2X).HasColumnName("LLead2X");

            entity.Property(e => e.Llead3A).HasColumnName("LLead3A");

            entity.Property(e => e.Llead3B).HasColumnName("LLead3B");

            entity.Property(e => e.Llead3C).HasColumnName("LLead3C");

            entity.Property(e => e.Llead3X).HasColumnName("LLead3X");

            entity.Property(e => e.Llead4A).HasColumnName("LLead4A");

            entity.Property(e => e.Llead4B).HasColumnName("LLead4B");

            entity.Property(e => e.Llead4C).HasColumnName("LLead4C");

            entity.Property(e => e.Llead4X).HasColumnName("LLead4X");

            entity.Property(e => e.MrpchangeDays).HasColumnName("MRPChangeDays");

            entity.Property(e => e.Olead1A).HasColumnName("OLead1A");

            entity.Property(e => e.Olead1B).HasColumnName("OLead1B");

            entity.Property(e => e.Olead1C).HasColumnName("OLead1C");

            entity.Property(e => e.Olead1X).HasColumnName("OLead1X");

            entity.Property(e => e.Olead2A).HasColumnName("OLead2A");

            entity.Property(e => e.Olead2B).HasColumnName("OLead2B");

            entity.Property(e => e.Olead2C).HasColumnName("OLead2C");

            entity.Property(e => e.Olead2X).HasColumnName("OLead2X");

            entity.Property(e => e.Olead3A).HasColumnName("OLead3A");

            entity.Property(e => e.Olead3B).HasColumnName("OLead3B");

            entity.Property(e => e.Olead3C).HasColumnName("OLead3C");

            entity.Property(e => e.Olead3X).HasColumnName("OLead3X");

            entity.Property(e => e.Olead4A).HasColumnName("OLead4A");

            entity.Property(e => e.Olead4B).HasColumnName("OLead4B");

            entity.Property(e => e.Olead4C).HasColumnName("OLead4C");

            entity.Property(e => e.Olead4X).HasColumnName("OLead4X");
        }
    }
}
