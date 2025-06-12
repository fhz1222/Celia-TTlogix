using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_PartMaster")]
    public class PartMaster
    {
        public string CustomerCode { get; set; }
        public string SupplierID { get; set; }
        public string ProductCode1 { get; set; }
        public string ProductCode2 { get; set; }
        public string ProductCode3 { get; set; }
        public string ProductCode4 { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        public string PackageType { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal SPQ { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal OrderLot { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal Length { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal Width { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal Height { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal NetWeight { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal GrossWeight { get; set; }
        public string OriginCountry { get; set; }
        public byte EnableSerialNo { get; set; }
        public byte IsStandardPackaging { get; set; }
        public byte Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string RevisedBy { get; set; }
        public DateTime? RevisedDate { get; set; }
        public byte IsDefected { get; set; }
        public byte IsPalletItem { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal CPartSPQ { get; set; }
        public byte IsCPart { get; set; }

        public bool MasterSlave { get; set; } = true;
        public bool BoxItem { get; set; }
        public int FloorStackability { get; set; } = 1;
        public int TruckStackability { get; set; } = 1;
        public int BoxesInPallet { get; set; } = 1;
        public bool DoNotSyncEDI { get; set; }
        public string iLogReadinessStatus { get; set; }
        public bool IsMixed { get; set; }
        public int? UnloadingPointId { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal LengthTT { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal WidthTT { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal HeightTT { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal NetWeightTT { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal GrossWeightTT { get; set; }
        public int PalletTypeId { get; set; }
        public int ELLISPalletTypeId { get; set; }
    }


}

