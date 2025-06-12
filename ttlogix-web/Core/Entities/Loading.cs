using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TT.Core.Enums;

namespace TT.Core.Entities
{
    [Table("TT_Loading")]
    public class Loading
    {
        [Key]
        [Column(TypeName="varchar(15)")]
        public string JobNo { get; set; }
        [Column(TypeName="varchar(10)")]
        public string CustomerCode { get; set; }
        [Column(TypeName="varchar(7)")]
        public string WHSCode { get; set; }
        [Column(TypeName="varchar(30)")]
        public string RefNo { get; set; }
        [Column(TypeName="varchar(100)")]
        public string Remark { get; set; } = string.Empty;
        public LoadingStatus Status { get; set; }
        [Column(TypeName="varchar(10)")]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [Column(TypeName="varchar(10)")]
        public string RevisedBy { get; set; }
        public DateTime? RevisedDate { get; set; }
        [Column(TypeName="varchar(10)")]
        public string CancelledBy { get; set; } = string.Empty;
        public DateTime? CancelledDate { get; set; }
        [Column(TypeName="varchar(10)")]
        public string ConfirmedBy { get; set; } = string.Empty;
        public DateTime? ConfirmedDate { get; set; }
        public DateTime? ETD { get; set; }
        public int? NoOfPallet { get; set; } = 0;
        public DateTime? TruckArrivalDate { get; set; }
        public DateTime? TruckDepartureDate { get; set; }
        public DateTime? ETA { get; set; }
        [Column(TypeName="varchar(99)")]
        public string TruckLicencePlate { get; set; }
        [Column(TypeName="varchar(8)")]
        public string TrailerNo { get; set; }
        [Column(TypeName="varchar(3)")]
        public string DockNo { get; set; }
        [Column(TypeName="varchar(99)")]
        public string TruckSeqNo { get; set; }
        public bool AllowedForDispatch { get; set; }
        public DateTime? AllowedForDispatchModifiedDate { get; set; }
        [Column(TypeName="varchar(10)")]
        public string AllowedForDispatchModifiedBy { get; set; }
    }
}
