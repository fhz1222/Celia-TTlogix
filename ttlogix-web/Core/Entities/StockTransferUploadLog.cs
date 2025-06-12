using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_StockTransferUploadLog")]
    public class StockTransferUploadLog
    {
        public string UPLJobNo { get; set; }
        public int LineItem { get; set; }
        public string StockTransferJobNo { get; set; }
        public string PID { get; set; }
        public string WHSCode { get; set; }
        public string LocationCode { get; set; }
        public string ScannedBy { get; set; }
        public DateTime UploadTime { get; set; }
        public byte Flag { get; set; }
        public string Remark { get; set; } = string.Empty;
        public string SysException { get; set; } = string.Empty;
    }
}


