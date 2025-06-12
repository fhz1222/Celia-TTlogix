namespace TT.Services.Models
{
    public class PickEntryDto
    {
        public bool IsNew { get; set; }
        public string JobNo { get; set; }
        public string ProductCode { get; set; }
        public string SupplierID { get; set; }
        public int LineItem { get; set; }
        public decimal QtyToPick { get; set; }
        public decimal AvailableQty { get; set; }
    }
}
