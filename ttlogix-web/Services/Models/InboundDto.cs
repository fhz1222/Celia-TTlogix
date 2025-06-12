using TT.Core.QueryResults;

namespace TT.Services.Models
{
    public class InboundDto : InboundWithExtendedDataQueryResult
    {
        public string StatusString => Status.ToString();
        public string TransTypeString => TransType.ToString();
    }
}
