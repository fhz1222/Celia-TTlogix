namespace Persistence.Entities;

public partial class EKanbanDetail
{
    public string OrderNo { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
    public string SerialNo { get; set; } = null!;
    public string SupplierId { get; set; } = null!;
    public string DropPoint { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal QuantitySupplied { get; set; }
    public decimal QuantityReceived { get; set; }
    public string BillingNo { get; set; } = null!;
    public int? ExternalLineItem { get; set; }
}
