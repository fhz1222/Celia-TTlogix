using System.Linq;
using TT.Core.QueryResults;

namespace TT.Services.Models
{
    public class InboundListItemDto : InboundListQueryResult
    {
        public string ContainerNo => ContainerNos != null ? string.Join(", ", ContainerNos.Distinct()) : string.Empty;
        public string TransTypeString => TransType.ToString();
        public string StatusString => Status.ToString();
    }
}
