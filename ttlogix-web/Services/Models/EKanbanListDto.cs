using System.Collections.Generic;

namespace TT.Services.Models
{
    public class EKanbanListDto : ListDtoBase
    {
        public IEnumerable<EKanbanListItemDto> Data { get; set; }
    }
}
