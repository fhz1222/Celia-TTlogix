using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using TT.Core.Enums;

namespace TT.Core.Entities
{
    [Table("TT_StorageDetail")]
    public class StorageDetail
    {
        [Key]
        public string PID { get; set; }
        public string InJobNo { get; set; }
        public int LineItem { get; set; }
        public int SeqNo { get; set; }
        public string ParentID { get; set; } = string.Empty;
        public string ProductCode { get; set; }
        public string CustomerCode { get; set; }
        public DateTime InboundDate { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal OriginalQty { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal Qty { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal QtyPerPkg { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal AllocatedQty { get; set; }
        public string OutJobNo { get; set; } = string.Empty;
        public int NoOfLabel { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal Length { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal Width { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal Height { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal NetWeight { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal GrossWeight { get; set; }
        public string ControlCode1 { get; set; } = string.Empty;
        public string ControlCode2 { get; set; } = string.Empty;
        public string ControlCode3 { get; set; } = string.Empty;
        public string ControlCode4 { get; set; } = string.Empty;
        public string ControlCode5 { get; set; } = string.Empty;
        public string ControlCode6 { get; set; } = string.Empty;
        public DateTime? ControlDate { get; set; }
        public string WHSCode { get; set; }
        public string LocationCode { get; set; } = string.Empty;
        public string SerialNo { get; set; } = string.Empty;
        public DateTime? ChargedDate { get; set; }
        public StorageStatus Status { get; set; }
        public string DownloadBy { get; set; } = string.Empty;
        public DateTime? DownloadDate { get; set; }
        public string PutawayBy { get; set; } = string.Empty;
        public DateTime? PutawayDate { get; set; }
        public string SupplierID { get; set; }
        public byte IsVMI { get; set; }
        public int Version { get; set; }
        public byte BondedStatus { get; set; }
        public Ownership Ownership { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal? BuyingPrice { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal? SellingPrice { get; set; }
        public string GroupID{get; set;}
    }

}
