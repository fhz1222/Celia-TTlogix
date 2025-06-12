namespace TT.Services.Models
{
    public class PickingListSummaryDto
    {
        public string JobNo { get; set; }
        public int LineItem { get; set; }
        public string ProductCode { get; set; }
        public decimal Qty { get; set; }
        public int Pkg { get; set; }
        public decimal PickedQty { get; set; }
        public int PickedPkg { get; set; }
    }
}
