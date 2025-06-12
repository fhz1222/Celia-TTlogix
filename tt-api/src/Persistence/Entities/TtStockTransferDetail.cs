namespace Persistence.Entities
{
    public partial class TtStockTransferDetail
    {
        public string JobNo { get; set; } = null!;
        public int LineItem { get; set; }
        public string Pid { get; set; } = null!;
        public string OriginalSupplierId { get; set; } = null!;
        public string OriginalWhscode { get; set; } = null!;
        public string OriginalLocationCode { get; set; } = null!;
        public string Whscode { get; set; } = null!;
        public string LocationCode { get; set; } = null!;
        public string TransferredBy { get; set; } = null!;
        public DateTime? TransferredDate { get; set; }
        public decimal? Qty { get; set; }
    }
}
