
namespace Persistence.Entities
{
    public partial class TtAddressContact
    {
        public string Code { get; set; } = null!;
        public string AddressCode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? TelNo { get; set; }
        public string? FaxNo { get; set; }
        public string Email { get; set; } = null!;
        public byte Status { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string? CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string? RevisedBy { get; set; }
        public DateTime? RevisedDate { get; set; }
    }
}
