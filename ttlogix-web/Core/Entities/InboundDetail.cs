using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_InboundDetail")]
    public  class InboundDetail
    {
        [Column(TypeName="varchar(15)")]
        public string JobNo { get; set; }
        public int LineItem { get; set; }
        public int PkgLineItem { get; set; }
        [Required]
        [Column(TypeName="varchar(30)")]
        public string ProductCode { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,6)")]
        public decimal ImportedQty { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal Qty { get; set; }
        public int NoOfPackage { get; set; } = 1;
        [Required]
        [Column(TypeName="varchar(7)")]
        public string PackageType { get; set; } = string.Empty;
        public int NoOfLabel { get; set; } = 1;
        [Column(TypeName = "numeric(18,6)")]
        public decimal Length { get; set; } = 1;
        [Column(TypeName = "numeric(18,6)")]
        public decimal Width { get; set; } = 1;
        [Column(TypeName = "numeric(18,6)")]
        public decimal Height { get; set; } = 1;
        [Column(TypeName = "numeric(18,6)")]
        public decimal NetWeight { get; set; } = 1;
        [Column(TypeName = "numeric(18,6)")]
        public decimal GrossWeight { get; set; } = 1;
        [Required]
        [Column(TypeName="varchar(30)")]
        public string ControlCode1 { get; set; } = string.Empty;
        [Required]
        [Column(TypeName="varchar(30)")]
        public string ControlCode2 { get; set; } = string.Empty;
        [Required]
        [Column(TypeName="varchar(30)")]
        public string ControlCode3 { get; set; } = string.Empty;
        [Required]
        [Column(TypeName="varchar(30)")]
        public string ControlCode4 { get; set; } = string.Empty;
        [Required]
        [Column(TypeName="varchar(30)")]
        public string ControlCode5 { get; set; } = string.Empty;
        [Required]
        [Column(TypeName="varchar(30)")]
        public string ControlCode6 { get; set; } = string.Empty;
        public DateTime? ControlDate { get; set; }
        [Column(TypeName="varchar(10)")]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [Column(TypeName="varchar(10)")]
        public string RevisedBy { get; set; } = string.Empty;
        public DateTime? RevisedDate { get; set; }
        [Column(TypeName="varchar(25)")]
        public string ASNNo { get; set; } = string.Empty;
        public int? ASNLineItem { get; set; } = 0;
        [Column(TypeName="varchar(100)")]
        public string Remark { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,6)")]
        public decimal? BuyingPricePerLine { get; set; } = 0;
    }

}
