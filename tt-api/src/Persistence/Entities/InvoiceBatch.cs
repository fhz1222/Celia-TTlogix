namespace Persistence.Entities;

public class InvoiceBatch
{
    public int Id { get; set; }
    public string SupplierId { get; set; } = null!;
    public string BatchNumber { get; set; } = null!;
    public string FactoryId { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime? ApprovedDate { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? RejectedDate { get; set; }
    public string? RejectedBy { get; set; }
}
