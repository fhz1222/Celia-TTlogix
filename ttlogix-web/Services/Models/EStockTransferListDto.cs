using System.Collections.Generic;

namespace TT.Services.Models
{
    public class EStockTransferListDto : ListDtoBase
    {
        public IEnumerable<EStockTransferListItemDto> Data { get; set; }
    }
}
