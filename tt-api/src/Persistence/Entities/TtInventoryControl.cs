namespace Persistence.Entities
{
    public partial class TtInventoryControl
    {
        public string CustomerCode { get; set; } = null!;
        public string Pc1type { get; set; } = null!;
        public string Pc2type { get; set; } = null!;
        public string Pc3type { get; set; } = null!;
        public string Pc4type { get; set; } = null!;
        public string Cc1type { get; set; } = null!;
        public string Cc2type { get; set; } = null!;
        public string Cc3type { get; set; } = null!;
        public string Cc4type { get; set; } = null!;
        public string Cc5type { get; set; } = null!;
        public string Cc6type { get; set; } = null!;
        public string? RevisedBy { get; set; }
        public DateTime? RevisedDate { get; set; }
        public string? SelectControlCode { get; set; }
    }
}
