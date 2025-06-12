namespace TT.Services.Models
{
    public class PriceMasterInboundUpdateDto
    {
        public UpdatePriceMasterInboundDetailsDto[] InboundDetailForPriceMasterDtos { get; set; }
        public string CustomerCode { get; set; }
        public string JobNo { get; set; }
        public string SupplierID { get; set; }
        public string Currency { get; set; }
    }
}
