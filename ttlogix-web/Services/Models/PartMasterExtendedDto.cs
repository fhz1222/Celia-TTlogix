namespace TT.Services.Models
{
    public class PartMasterExtendedDto
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
        public decimal SPQ { get; set; }
        public byte IsPalletItem { get; set; }
        public decimal CPartSPQ { get; set; }
        public byte IsCPart { get; set; }
        public decimal AvailableQty { get; set; }
        public decimal SupplierQty { get; set; }
        public decimal EHPQty { get; set; }
    }
}
