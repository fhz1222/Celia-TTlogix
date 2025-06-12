using TT.Core.Enums;

namespace TT.Core.Extensions
{
    public static class EntityExtensions
    {
        public static LoadingStatus GetLoadingCalculatedStatus(OutboundStatus minOutboundStatus, OutboundStatus maxOutboundStatus)
        {
            return (minOutboundStatus >= OutboundStatus.Picked && maxOutboundStatus < OutboundStatus.Cancelled) ? LoadingStatus.Picked :
            (minOutboundStatus == OutboundStatus.NewJob && maxOutboundStatus == OutboundStatus.NewJob) ? LoadingStatus.NewJob :
            (minOutboundStatus >= OutboundStatus.NewJob && maxOutboundStatus < OutboundStatus.Cancelled) ? LoadingStatus.Processing :
            LoadingStatus.NewJob;

        }
    }
}
