using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("EKanbanHeader")]
    public class EKanbanHeader
    {
        [Key]
        public string OrderNo { get; set; } = string.Empty;
        public string FactoryID { get; set; } = string.Empty;
        public string RunNo { get; set; } = string.Empty;
        public DateTime IssuedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public string Instructions { get; set; } = string.Empty;
        public byte Status { get; set; }
        public string OutJobNo { get; set; } = string.Empty;
        public DateTime? ETA { get; set; }
        public string BlanketOrderNo { get; set; } = string.Empty;
        public string RefNo { get; set; } = string.Empty;
    }
}
