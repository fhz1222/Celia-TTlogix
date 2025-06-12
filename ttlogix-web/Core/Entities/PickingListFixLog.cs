using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_PickingListFixLog")]
    public class PickingListFixLog
    {
        public string JobNo { get; set; }
        public string PID { get; set; }
        public string ProductCode { get; set; }
        public string SupplierID { get; set; }
        public string ParentPID { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal OriginalQty { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal Qty { get; set; }
        public int ItemType { get; set; }
        public int Split { get; set; }
        public int Destination { get; set; }
        public string FixDescription { get; set; }
        public string FixedBy { get; set; }
        public DateTime FixedDate { get; set; }
        public int LineItem { get; set; }
        public int SeqNo { get; set; }
        public DateTime InboundDate { get; set; }
        public string WHSCode { get; set; }
        public string LocationCode { get; set; }
        public string PackageID { get; set; }
        public DateTime? ControlDate { get; set; }
        public byte? ControlCodeType { get; set; }
        public string ControlCode { get; set; }
        public string ControlCodeValue { get; set; }
        public string DropPoint { get; set; }
        public string ProductionLine { get; set; }
        public int? Version { get; set; }
        public string DownloadBy { get; set; }
        public DateTime? DownloadDate { get; set; }
        public string PickedBy { get; set; }
        public DateTime? PickedDate { get; set; }
        public string PackedBy { get; set; }
        public DateTime? PackedDate { get; set; }
    }

}


