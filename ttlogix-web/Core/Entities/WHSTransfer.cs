using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_WHSTransfer")]
    public class WHSTransfer
    {
        [Key]
        public string JobNo { get; set; }
        public string CustomerCode { get; set; }
        public string WHSCode { get; set; }
        public string NewWHSCode { get; set; }
        public string Remark { get; set; }
        public byte Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string RevisedBy { get; set; }
        public DateTime? RevisedDate { get; set; }
        public string ConfirmedBy { get; set; }
        public DateTime? ConfirmedDate { get; set; }
        public string CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
    }

}


