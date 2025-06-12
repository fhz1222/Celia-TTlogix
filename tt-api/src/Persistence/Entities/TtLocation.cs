namespace Persistence.Entities
{
    public partial class TtLocation
    {
        public string Code { get; set; } = null!;
        public string Whscode { get; set; } = null!;
        public string AreaCode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal M3 { get; set; }
        public byte Status { get; set; }
        public byte Type { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string? CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
        public byte? IsPriority { get; set; }
        public int ILogLocationCategoryId { get; set; }
    }
}
