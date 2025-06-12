using System.Collections.Generic;

namespace TT.Services.Models
{
    public class LoadingListDto : ListDtoBase
    {
        public IEnumerable<LoadingListItemDto> Data { get; set; }
    }
}
