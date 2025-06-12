using PetaPoco;

namespace Persistence.PetaPoco.Models
{
    [TableName("[dbo].[TT_PartMaster]")]
    [PrimaryKey("Code", AutoIncrement = false)]
    [ExplicitColumns]
    public partial class TT_PartMaster
    {
        public const string SqlTableName = "TT_PartMaster";
        [Column]
        public string CustomerCode { get; set; } = null!;
        [Column]
        public string SupplierId { get; set; } = null!;
        [Column]
        public string ProductCode1 { get; set; } = null!;
        [Column]
        public string ProductCode2 { get; set; } = null!;
        [Column]
        public string ProductCode3 { get; set; } = null!;
        [Column]
        public string ProductCode4 { get; set; } = null!;
        [Column]
        public string Description { get; set; } = null!;
        [Column]
        public string Uom { get; set; } = null!;
        [Column]
        public string PackageType { get; set; } = null!;
        [Column]
        public decimal Spq { get; set; }
        [Column]
        public decimal OrderLot { get; set; }
        [Column]
        public decimal Length { get; set; }
        [Column]
        public decimal Width { get; set; }
        [Column]
        public decimal Height { get; set; }
        [Column]
        public decimal NetWeight { get; set; }
        [Column]
        public decimal GrossWeight { get; set; }
        [Column]
        public string OriginCountry { get; set; } = null!;
        [Column]
        public byte EnableSerialNo { get; set; }
        [Column]
        public byte IsStandardPackaging { get; set; }
        [Column]
        public byte Status { get; set; }
        [Column]
        public string CreatedBy { get; set; } = null!;
        [Column]
        public DateTime? CreatedDate { get; set; }
        [Column]
        public string? RevisedBy { get; set; }
        [Column]
        public DateTime? RevisedDate { get; set; }
        [Column]
        public byte IsDefected { get; set; }
        [Column]
        public byte IsPalletItem { get; set; }
        [Column]
        public decimal CpartSpq { get; set; }
        [Column]
        public byte IsCpart { get; set; }
        [Column]
        public bool? MasterSlave { get; set; }
        [Column]
        public bool BoxItem { get; set; }
        [Column]
        public int FloorStackability { get; set; }
        [Column]
        public int TruckStackability { get; set; }
        [Column]
        public int BoxesInPallet { get; set; }
        [Column]
        public bool DoNotSyncEdi { get; set; }
    }
}
