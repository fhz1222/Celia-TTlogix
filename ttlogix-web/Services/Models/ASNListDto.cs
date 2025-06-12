using System.Collections.Generic;

namespace TT.Services.Models
{
    public class ASNListDto : ListDtoBase
    {
        public IEnumerable<ASNListItemDto> Data { get; set; }
    }
}
