namespace TT.Services.Models
{
    public class PriceMasterOutboundUpdateDto
    {
        public UpdatePriceMasterPickingListDto[] PickingListForPriceMasterDtos { get; set; }
        public string CustomerCode { get; set; }
        public string JobNo { get; set; }
    }
}
