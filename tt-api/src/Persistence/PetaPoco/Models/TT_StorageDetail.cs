using PetaPoco;

namespace Persistence.PetaPoco.Models
{
    [TableName("[dbo].[TT_StorageDetail]")]
    [PrimaryKey("Code", AutoIncrement = false)]
    [ExplicitColumns]
    public partial class TT_StorageDetail
    {
        public const string SqlTableName = "TT_StorageDetail";
        [Column]
        public string PID { get; set; } = null!;
        [Column] 
        public string InJobNo { get; set; } = null!;
        [Column]
        public int LineItem { get; set; }
        [Column]
        public int SeqNo { get; set; }
        [Column]
        public string ParentId { get; set; } = null!;
        [Column]
        public string ProductCode { get; set; } = null!;
        [Column]
        public string? CustomerCode { get; set; }
        [Column]
        public DateTime InboundDate { get; set; }
        [Column]
        public decimal OriginalQty { get; set; }
        [Column]
        public decimal Qty { get; set; }
        [Column]
        public decimal QtyPerPkg { get; set; }
        [Column]
        public decimal AllocatedQty { get; set; }
        [Column]
        public string OutJobNo { get; set; } = null!;
        [Column]
        public int NoOfLabel { get; set; }
        [Column]
        public decimal Length { get; set; }
        [Column]
        public decimal Width { get; set; }
        [Column]
        public decimal Height { get; set; }
        [Column]
        public decimal NetWeight { get; set; }
        [Column]
        public decimal GrossWeight { get; set; }
        [Column]
        public string ControlCode1 { get; set; } = null!;
        [Column]
        public string ControlCode2 { get; set; } = null!;
        [Column]
        public string ControlCode3 { get; set; } = null!;
        [Column]
        public string ControlCode4 { get; set; } = null!;
        [Column]
        public string ControlCode5 { get; set; } = null!;
        [Column]
        public string ControlCode6 { get; set; } = null!;
        [Column]
        public DateTime? ControlDate { get; set; }
        [Column]
        public string WHSCode { get; set; } = null!;
        [Column]
        public string LocationCode { get; set; } = null!;
        [Column]
        public string SerialNo { get; set; } = null!;
        [Column]
        public DateTime? ChargedDate { get; set; }
        [Column]
        public byte Status { get; set; }
        [Column]
        public string? DownloadBy { get; set; }
        [Column]
        public DateTime? DownloadDate { get; set; }
        [Column]
        public string PutawayBy { get; set; } = null!;
        [Column]
        public DateTime? PutawayDate { get; set; }
        [Column]
        public string SupplierId { get; set; } = null!;
        [Column]
        public byte IsVmi { get; set; }
        [Column]
        public int Version { get; set; }
        [Column]
        public byte BondedStatus { get; set; }
        [Column]
        public byte Ownership { get; set; }
        [Column]
        public decimal? BuyingPrice { get; set; }
        [Column]
        public decimal? SellingPrice { get; set; }
        [Column]
        public string? GroupId { get; set; }
    }
}
