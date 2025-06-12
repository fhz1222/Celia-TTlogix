using System.Collections.Generic;

namespace TT.Services.Models
{
    public class UpdateSellingPriceItemDto
    {
        [Services.Utilities.RequiredAsJsonError]
        public string PID { get; set; }
        [Services.Utilities.RequiredAsJsonError]
        public decimal Price { get; set; }
    }
    public class UpdateSellingPriceItemsDto
    {
        [Services.Utilities.RequiredAsJsonError]
        public IEnumerable<UpdateSellingPriceItemDto> Data { get; set; }
    }
}
