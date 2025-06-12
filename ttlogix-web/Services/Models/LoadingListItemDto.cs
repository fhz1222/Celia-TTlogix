using TT.Core.QueryResults;

namespace TT.Services.Models
{
    public class LoadingListItemDto : LoadingListQueryResult
    {
        public string StatusString => Status.ToString();
        public string CalculatedStatusString => CalculatedStatus.ToString();
    }
}
