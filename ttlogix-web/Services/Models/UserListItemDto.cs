using TT.Core.QueryResults;

namespace TT.Services.Models
{
    public class UserListItemDto : UserListQueryResult
    {
        public string StatusString => Status.ToString();
    }
}
