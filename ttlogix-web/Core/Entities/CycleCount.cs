using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TT.Core.Enums;

namespace TT.Core.Entities
{
    [Table("TT_CycleCount")]
    public class CycleCount
    {
        [Key]
        public string JobNo { get; set; }
        public string CustomerCode { get; set; }
        public string Remark { get; set; }
        public string WHSCode { get; set; }
        public byte CountType { get; set; }
        public byte CountMethod { get; set; }
        public CycleCountStatus Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string RevisedBy { get; set; }
        public DateTime? RevisedDate { get; set; }
        public string CompletedBy { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
    }
}
