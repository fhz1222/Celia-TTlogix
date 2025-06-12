
namespace Persistence.Entities
{
    public partial class TtCountry
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public byte Status { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string? CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
    }
}
