using TT.Core.QueryResults;

namespace TT.Services.Models
{
    public class AccessGroupSimpleDto : AccessGroupSimpleQueryResult
    {
        public string StatusString => Status.ToString();
    }
}
