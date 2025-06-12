namespace Persistence.Entities
{
    public partial class TtLabelPrinter
    {
        public string Ip { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string? RevisedBy { get; set; }
        public DateTime? RevisedDate { get; set; }
        public int Type { get; set; }
    }
}
