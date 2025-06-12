using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TT.Core.Enums;

namespace TT.Core.Entities
{
    [Table("EStockTransferHeader")]
    public class EStockTransferHeader
    {
        [Key]
        public string OrderNo { get; set; }
        public string FactoryID { get; set; }
        public string RunNo { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public string Instructions { get; set; }
        public EStockTransferStatus Status { get; set; }
        public string StockTransferJobNo { get; set; }
        public DateTime? ETA { get; set; }
        public string BlanketOrderNo { get; set; }
        public string RefNo { get; set; }
    }
}


