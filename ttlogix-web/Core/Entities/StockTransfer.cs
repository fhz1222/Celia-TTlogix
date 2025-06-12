using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TT.Core.Enums;

namespace TT.Core.Entities
{
    [Table("TT_StockTransfer")]
    public class StockTransfer
    {
        [Key]
        public string JobNo { get; set; }
        public string CustomerCode { get; set; }
        public string WHSCode { get; set; }
        public string RefNo { get; set; } = string.Empty;
        public string Remark { get; set; } = string.Empty;
        public StockTransferType TransferType { get; set; }
        public StockTransferStatus Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string RevisedBy { get; set; } = string.Empty;
        public DateTime? RevisedDate { get; set; }
        public string ConfirmedBy { get; set; } = string.Empty;
        public DateTime? ConfirmedDate { get; set; }
        public string CancelledBy { get; set; } = string.Empty;
        public DateTime? CancelledDate { get; set; }
        public string CommInvNo { get; set; }
        public DateTime? CommInvDate { get; set; }
        public byte? DESADV { get; set; }
    }

}


