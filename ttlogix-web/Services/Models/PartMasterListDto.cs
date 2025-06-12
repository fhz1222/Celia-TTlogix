using System.Collections.Generic;

namespace TT.Services.Models
{
    public class PartMasterListDto : ListDtoBase
    {
        public IEnumerable<PartMasterListItemDto> Data { get; set; }
    }
}
