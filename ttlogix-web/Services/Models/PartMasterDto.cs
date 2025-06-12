using TT.Core.Enums;

namespace TT.Services.Models
{
    public class PartMasterDto
    {
        public string CustomerCode { get; set; }
        public string SupplierID { get; set; }
        public string ProductCode1 { get; set; }
        public string ProductCode2 { get; set; }
        public string ProductCode3 { get; set; }
        public string ProductCode4 { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        public string OriginCountry { get; set; }
        public ValueStatus Status { get; set; }
        public bool EnableSerialNo { get; set; }
        public string PackageType { get; set; }
        public bool IsStandardPackaging { get; set; }
        public bool IsDefected { get; set; }
        public decimal SPQ { get; set; }
        public decimal OrderLot { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal NetWeight { get; set; }
        public decimal GrossWeight { get; set; }
        public bool IsPalletItem { get; set; }
        public bool IsCPart { get; set; }
        public decimal CPartSPQ { get; set; }
        public bool MasterSlave { get; set; } = true;
        public bool BoxItem { get; set; }
        public int FloorStackability { get; set; } = 1;
        public int TruckStackability { get; set; } = 1;
        public int BoxesInPallet { get; set; } = 1;
        public bool DoNotSyncEDI { get; set; }
        public string SupplierName { get; set; }
        public string iLogReadinessStatus { get; set; }
        public bool IsMixed { get; set; }
        public int? UnloadingPointId { get; set; }
        public decimal LengthInternal { get; set; }
        public decimal WidthInternal { get; set; }
        public decimal HeightInternal { get; set; }
        public decimal NetWeightInternal { get; set; }
        public decimal GrossWeightInternal { get; set; }
        public int PalletTypeId { get; set; }
        public int ELLISPalletTypeId { get; set; }

        public bool SPQCorrectForCPart => !IsCPart || (CPartSPQ > 0 && SPQ % CPartSPQ == 0);
    }
}
