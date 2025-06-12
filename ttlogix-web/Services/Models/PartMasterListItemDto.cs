using TT.Core.QueryResults;

namespace TT.Services.Models
{
    public class PartMasterListItemDto : PartMasterListQueryResult
    {
        public string StatusString => Status.ToString();
    }
}
