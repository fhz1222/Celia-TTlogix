namespace Persistence.Entities
{
    public partial class TtPartMaster
    {
        public string CustomerCode { get; set; } = null!;
        public string SupplierId { get; set; } = null!;
        public string ProductCode1 { get; set; } = null!;
        public string ProductCode2 { get; set; } = null!;
        public string ProductCode3 { get; set; } = null!;
        public string ProductCode4 { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Uom { get; set; } = null!;
        public string PackageType { get; set; } = null!;
        public decimal Spq { get; set; }
        public decimal OrderLot { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal NetWeight { get; set; }
        public decimal GrossWeight { get; set; }
        public string OriginCountry { get; set; } = null!;
        public byte EnableSerialNo { get; set; }
        public byte IsStandardPackaging { get; set; }
        public byte Status { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? CreatedDate { get; set; }
        public string? RevisedBy { get; set; }
        public DateTime? RevisedDate { get; set; }
        public byte IsDefected { get; set; }
        public byte IsPalletItem { get; set; }
        public decimal CpartSpq { get; set; }
        public byte IsCpart { get; set; }
        public bool IsMixed { get; set; }
        public bool? MasterSlave { get; set; }
        public bool BoxItem { get; set; }
        public int FloorStackability { get; set; }
        public int TruckStackability { get; set; }
        public int BoxesInPallet { get; set; }
        public bool DoNotSyncEdi { get; set; }
        public int? UnloadingPointId { get; set; }
        public decimal LengthTt { get; set; }
        public decimal WidthTt { get; set; }
        public decimal HeightTt { get; set; }
        public decimal NetWeightTt { get; set; }
        public decimal GrossWeightTt { get; set; }
        public int PalletTypeId { get; set; }
    }
}
