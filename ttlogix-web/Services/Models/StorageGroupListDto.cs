using System.Collections.Generic;

namespace TT.Services.Models
{
    public class StorageGroupListDto : ListDtoBase
    {
        public IEnumerable<StorageGroupDto> Data { get; set; }
    }
}
