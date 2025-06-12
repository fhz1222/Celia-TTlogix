using System.Collections.Generic;

namespace TT.Services.Models
{
    public class InboundListDto : ListDtoBase
    {
        public IEnumerable<InboundListItemDto> Data { get; set; }
    }
}
