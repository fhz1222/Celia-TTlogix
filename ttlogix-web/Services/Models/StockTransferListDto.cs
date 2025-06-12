using System.Collections.Generic;

namespace TT.Services.Models
{
    public class StockTransferListDto : ListDtoBase
    {
        public IEnumerable<StockTransferListItemDto> Data { get; set; }
    }
}
