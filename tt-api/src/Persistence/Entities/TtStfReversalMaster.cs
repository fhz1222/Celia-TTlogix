namespace Persistence.Entities
{
    public partial class TtStfReversalMaster
    {
        public string JobNo { get; set; } = null!;
        public string CustomerCode { get; set; } = null!;
        public string Whscode { get; set; } = null!;
        public string RefNo { get; set; } = null!;
        public string StfjobNo { get; set; } = null!;
        public string? Reason { get; set; }
        public byte Status { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string? ConfirmedBy { get; set; }
        public DateTime? ConfirmedDate { get; set; }
        public string? CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
    }
}
