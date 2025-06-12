using TT.Core.Enums;
using TT.Core.QueryResults;

namespace TT.Services.Models
{
    public class OutboundDetailDto : OutboundDetailQueryResult
    {
        public OutboundStatus StatusValue
        {
            get => (OutboundStatus)Status;
            set => Status = (byte)value;
        }
    }
}
