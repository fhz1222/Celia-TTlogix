using System.Collections.Generic;

namespace TT.Services.Models
{
    public class UserListDto : ListDtoBase
    {
        public IEnumerable<UserListItemDto> Data { get; set; }
    }
}
