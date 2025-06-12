using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_PickingList")]
    public class PickingList
    {
        public string JobNo { get; set; } = string.Empty;
        public int LineItem { get; set; }
        public int SeqNo { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string SupplierID { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,6)")]
        public decimal Qty { get; set; }
        public string PID { get; set; } = string.Empty;
        public string WHSCode { get; set; } = string.Empty;
        public string LocationCode { get; set; } = string.Empty;
        public string PackageID { get; set; } = string.Empty;
        public DateTime InboundDate { get; set; }
        public DateTime? ControlDate { get; set; }
        public byte? ControlCodeType { get; set; } = 0;
        public string ControlCode { get; set; } = string.Empty;
        public string ControlCodeValue { get; set; } = string.Empty;
        public string DropPoint { get; set; } = string.Empty;
        public string ProductionLine { get; set; } = string.Empty;
        public int? Version { get; set; } = 0;
        public string DownloadBy { get; set; } = string.Empty;
        public DateTime? DownloadDate { get; set; }
        public string PickedBy { get; set; } = string.Empty;
        public DateTime? PickedDate { get; set; }
        public string PackedBy { get; set; } = string.Empty;
        public DateTime? PackedDate { get; set; }
        public string InboundJobNo { get; set; } = string.Empty;
        public string AllocatedPid { get; set; }
    }
}
