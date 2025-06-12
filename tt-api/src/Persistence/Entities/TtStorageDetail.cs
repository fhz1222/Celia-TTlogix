namespace Persistence.Entities
{
    public partial class TtStorageDetail
    {
        public string Pid { get; set; } = null!;
        public string InJobNo { get; set; } = null!;
        public int LineItem { get; set; }
        public int SeqNo { get; set; }
        public string ParentId { get; set; } = null!;
        public string ProductCode { get; set; } = null!;
        public string? CustomerCode { get; set; }
        public DateTime InboundDate { get; set; }
        public decimal OriginalQty { get; set; }
        public decimal Qty { get; set; }
        public decimal QtyPerPkg { get; set; }
        public decimal AllocatedQty { get; set; }
        public string OutJobNo { get; set; } = null!;
        public int NoOfLabel { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal NetWeight { get; set; }
        public decimal GrossWeight { get; set; }
        public string ControlCode1 { get; set; } = null!;
        public string ControlCode2 { get; set; } = null!;
        public string ControlCode3 { get; set; } = null!;
        public string ControlCode4 { get; set; } = null!;
        public string ControlCode5 { get; set; } = null!;
        public string ControlCode6 { get; set; } = null!;
        public DateTime? ControlDate { get; set; }
        public string Whscode { get; set; } = null!;
        public string LocationCode { get; set; } = null!;
        public string SerialNo { get; set; } = null!;
        public DateTime? ChargedDate { get; set; }
        public byte Status { get; set; }
        public string? DownloadBy { get; set; }
        public DateTime? DownloadDate { get; set; }
        public string PutawayBy { get; set; } = null!;
        public DateTime? PutawayDate { get; set; }
        public string SupplierId { get; set; } = null!;
        public byte IsVmi { get; set; }
        public int Version { get; set; }
        public byte BondedStatus { get; set; }
        public byte Ownership { get; set; }
        public decimal? BuyingPrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public string? GroupId { get; set; }
    }
}
