using System.Collections.Generic;

namespace TT.Services.Models
{
    public class UpdateBuyingPriceItemDto
    {
        public string InJobNo { get; set; }
        public string Currency { get; set; }
        public IEnumerable<BuyingPriceItem> Prices { get; set; }

        public class BuyingPriceItem
        {
            public int LineItem { get; set; }
            public decimal BuyingPrice { get; set; }
        }
    }
}
