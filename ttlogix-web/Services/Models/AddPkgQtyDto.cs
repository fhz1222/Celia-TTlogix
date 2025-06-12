namespace TT.Services.Models
{
    public class AddPkgQtyDto
    {
        [Services.Utilities.RequiredAsJsonError]
        public string JobNo { get; set; }
        [Services.Utilities.RequiredAsJsonError]
        public int LineItem { get; set; }
        public decimal Qty { get; set; }
    }
}
