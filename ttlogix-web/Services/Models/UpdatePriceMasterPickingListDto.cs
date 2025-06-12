namespace TT.Services.Models
{
    public class UpdatePriceMasterPickingListDto
    {
        public string PID { get; set; }
        public string SupplierId { get; set; }
        public string ProductCode { get; set; }
        public decimal Price { get; set; }
    }
}
