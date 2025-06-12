
namespace Persistence.Entities
{
    public partial class TtAddressBook
    {
        public string Code { get; set; } = null!;
        public string CompanyCode { get; set; } = null!;
        public byte Status { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string? Address4 { get; set; }
        public string? PostCode { get; set; }
        public string? Country { get; set; }
        public string? TelNo { get; set; }
        public string? FaxNo { get; set; }
        public string CreateBy { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string? CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string? RevisedBy { get; set; }
        public DateTime? RevisedDate { get; set; }
    }
}
