using System;

namespace TT.Core.QueryResults
{
    public class PartMasterBySupplierQueryResult
    {
        public string CustomerCode { get; set; }
        public string SupplierID { get; set; }
        public string ProductCode1 { get; set; }
        //public string ProductCode2 { get; set; }
        //public string ProductCode3 { get; set; }
        //public string ProductCode4 { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        public string PackageType { get; set; }
        public decimal SPQ { get; set; }
        //public decimal OrderLot { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal SizeM3 => Length * Width * Height / 1000000; 
        public decimal NetWeight { get; set; }
        public decimal GrossWeight { get; set; }
        public byte IsStandardPackaging { get; set; }
        public byte Status { get; set; }
        public byte IsDefected { get; set; }

        public string UOMName { get; set; }
        public string PkgTypeName { get; set; }
        public decimal DecimalNum { get; set; }
    }
}