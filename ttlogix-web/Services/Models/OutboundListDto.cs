using System.Collections.Generic;

namespace TT.Services.Models
{
    public class OutboundListDto : ListDtoBase
    {
        public IEnumerable<OutboundListItemDto> Data { get; set; }
    }
}
