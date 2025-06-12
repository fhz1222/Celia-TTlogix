using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TT.Core.Enums;

namespace TT.Core.Entities
{
    [Table("TT_Inbound")]
    public class Inbound
    {
        [Key]
        [Column(TypeName="varchar(15)")]
        public string JobNo { get; set; }
        [Column(TypeName="varchar(10)")]
        public string CustomerCode { get; set; } = string.Empty;
        [Column(TypeName="varchar(7)")]
        public string WHSCode { get; set; } = string.Empty;
        [Column(TypeName="varchar(25)")]
        public string IRNo { get; set; } = string.Empty;
        [Column(TypeName="varchar(30)")]
        public string RefNo { get; set; } = string.Empty;
        public DateTime ETA { get; set; }
        public InboundType TransType { get; set; }
        public byte Charged { get; set; }
        [Column(TypeName="varchar(100)")]
        public string Remark { get; set; } = string.Empty;
        public InboundStatus Status { get; set; }
        [Column(TypeName="varchar(10)")]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [Column(TypeName="varchar(10)")]
        public string RevisedBy { get; set; } = string.Empty;
        public DateTime? RevisedDate { get; set; }
        [Column(TypeName="varchar(10)")]
        public string CancelledBy { get; set; } = string.Empty;
        public DateTime? CancelledDate { get; set; }
        [Column(TypeName="varchar(10)")]
        public string PutawayBy { get; set; } = string.Empty;
        public DateTime? PutawayDate { get; set; }
        [Column(TypeName="varchar(10)")]
        public string SupplierID { get; set; } = string.Empty;
        [Column(TypeName="varchar(7)")]
        public string Currency { get; set; } = string.Empty;
        [Column(TypeName="varchar(18)")]
        public string IM4No { get; set; } = string.Empty;
        public DateTime? CustomsDeclarationDate { get; set; }
    }
}
