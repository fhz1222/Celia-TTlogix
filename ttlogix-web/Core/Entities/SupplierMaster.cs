using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TT.Core.Entities
{
    [Table("SupplierMaster")]
    public class SupplierMaster
    {
        public string FactoryID { get; set; }
        public string SupplierID { get; set; }
        public string CompanyName { get; set; }
        public string StreetAddress { get; set; }
        public string Suburb { get; set; }
        public string Country { get; set; }
        public string PostCode { get; set; }
        public string SupplyParadigm { get; set; }
        public string SourceOfParts { get; set; }
        public string AgreementCode { get; set; }
        public byte Status { get; set; }
        public byte IsBonded { get; set; }
        public byte SAPVendorType { get; set; }
        public string BlanketOrderNo { get; set; }
        public string RevisedBy { get; set; }
        public DateTime? RevisedDate { get; set; }
        public int? NoEDIFlag { get; set; }
        public bool IsVMI => SupplyParadigm?.ToUpper().EndsWith('V') == true;
    }
}

