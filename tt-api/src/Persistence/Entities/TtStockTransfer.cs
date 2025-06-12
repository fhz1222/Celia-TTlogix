namespace Persistence.Entities
{
    public partial class TtStockTransfer
    {
        public string JobNo { get; set; } = null!;
        public string CustomerCode { get; set; } = null!;
        public string WhsCode { get; set; } = null!;
        public string? RefNo { get; set; }
        public string? Remark { get; set; }
        public byte TransferType { get; set; }
        public byte Status { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string? RevisedBy { get; set; }
        public DateTime? RevisedDate { get; set; }
        public string? ConfirmedBy { get; set; }
        public DateTime? ConfirmedDate { get; set; }
        public string? CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string? CommInvNo { get; set; }
        public DateTime? CommInvDate { get; set; }
        public byte? Desadv { get; set; }
    }
}
