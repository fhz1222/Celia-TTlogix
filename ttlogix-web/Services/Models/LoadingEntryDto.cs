using TT.Core.Enums;
using TT.Core.QueryResults;

namespace TT.Services.Models
{
    public class LoadingEntryDto : LoadingEntryListQueryResult
    {
        public string OutboundStatusString => this.OutboundStatus != null ? ((OutboundStatus)OutboundStatus).ToString() : null;
    }
}
