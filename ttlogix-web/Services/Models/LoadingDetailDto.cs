using TT.Core.Enums;
using TT.Core.QueryResults;

namespace TT.Services.Models
{
    public class LoadingDetailDto : LoadingDetailQueryResult
    {
        public string OutboundStatusString => OutboundStatus.ToString();
    }
}
