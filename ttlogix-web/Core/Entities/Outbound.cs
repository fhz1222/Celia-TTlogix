using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using TT.Core.Enums;

namespace TT.Core.Entities
{
    [Table("TT_Outbound")]
    public class Outbound
    {
        [Key]
        [Column(TypeName="varchar(15)")]
        public string JobNo { get; set; }
        [Column(TypeName="varchar(10)")]
        public string CustomerCode { get; set; } = string.Empty;
        [Column(TypeName="varchar(7)")]
        public string WHSCode { get; set; } = string.Empty;
        [Column(TypeName="varchar(15)")]
        public string OSNo { get; set; } = string.Empty;
        [Column(TypeName="varchar(30)")]
        public string RefNo { get; set; } = string.Empty;
        public DateTime ETD { get; set; }
        public OutboundType TransType { get; set; }
        public byte Charged { get; set; }
        [Column(TypeName="varchar(100)")]
        public string Remark { get; set; } = string.Empty;
        public OutboundStatus Status { get; set; }
        [Column(TypeName="varchar(10)")]
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        [Column(TypeName="varchar(10)")]
        public string RevisedBy { get; set; } = string.Empty;
        public DateTime? RevisedDate { get; set; }
        [Column(TypeName="varchar(10)")]
        public string CancelledBy { get; set; } = string.Empty;
        public DateTime? CancelledDate { get; set; }
        [Column(TypeName="varchar(10)")]
        public string DispatchedBy { get; set; } = string.Empty;
        public DateTime? DispatchedDate { get; set; }
        [Column(TypeName="varchar(1292)")]
        public string CommInvNo { get; set; } = string.Empty;
        public int? NoOfPallet { get; set; } = 0;
        [Column(TypeName="varchar(20)")]
        public string DeliveryTo { get; set; } = string.Empty;
        [Column(TypeName="varchar(7)")]
        public string NewWHSCode { get; set; } = string.Empty;
        public bool XDock { get; set; }
        [Column(TypeName = "varchar(30)")]
        public string TransportNo { get; set; } = string.Empty;
    }
}
