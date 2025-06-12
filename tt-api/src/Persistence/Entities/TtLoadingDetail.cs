namespace Persistence.Entities
{
    public partial class TtLoadingDetail
    {
        public string JobNo { get; set; } = null!;
        public string OrderNo { get; set; } = null!;
        public string SupplierId { get; set; } = null!;
        public DateTime? Etd { get; set; }
        public string AddedBy { get; set; } = null!;
        public DateTime AddedDate { get; set; }
        public string? OutJobNo { get; set; }
    }
}
