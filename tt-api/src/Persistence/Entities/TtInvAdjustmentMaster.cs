namespace Persistence.Entities
{
    public partial class TtInvAdjustmentMaster
    {
        public string JobNo { get; set; } = null!;
        public string CustomerCode { get; set; } = null!;
        public string Whscode { get; set; } = null!;
        public string RefNo { get; set; } = null!;
        /// <summary>
        /// 0= Normal ; 1=Undo ZeroOut
        /// </summary>
        public byte JobType { get; set; }
        public string? Reason { get; set; }
        public byte Status { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string? RevisedBy { get; set; }
        public DateTime? RevisedDate { get; set; }
        public string? ConfirmedBy { get; set; }
        public DateTime? ConfirmedDate { get; set; }
        public string? CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
    }
}
