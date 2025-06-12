

using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.Entities
{
    public partial class SupplierMaster
    {
        public string FactoryId { get; set; } = null!;
        public string SupplierId { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string StreetAddress { get; set; } = null!;
        public string Suburb { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string PostCode { get; set; } = null!;
        public string SupplyParadigm { get; set; } = null!;
        public string SourceOfParts { get; set; } = null!;
        public string AgreementCode { get; set; } = null!;
        public byte Status { get; set; }
        public byte IsBonded { get; set; }
        public byte SapVendorType { get; set; }
        public string? BlanketOrderNo { get; set; }
        public string? RevisedBy { get; set; }
        public DateTime? RevisedDate { get; set; }
        public int? NoEDIFlag { get; set; }
    }
}
