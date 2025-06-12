namespace Persistence.Entities
{
    public partial class TtInvAdjustmentDetail
    {
        public string JobNo { get; set; } = null!;
        public int LineItem { get; set; }
        public string Pid { get; set; } = null!;
        public string ProductCode { get; set; } = null!;
        public byte Act { get; set; }
        public double OldQty { get; set; }
        public double NewQty { get; set; }
        public double OldQtyPerPkg { get; set; }
        public double NewQtyPerPkg { get; set; }
        public string? Remark { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string? RevisedBy { get; set; }
        public DateTime? RevisedDate { get; set; }
    }
    public enum InventoryAdjustmentType
    {
        Negative = 0,
        Positive = 1
    }
}
