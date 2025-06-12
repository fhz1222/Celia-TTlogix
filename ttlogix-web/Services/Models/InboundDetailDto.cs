using TT.Core.QueryResults;

namespace TT.Services.Models
{
    public class InboundDetailDto : InboundDetailQueryResult
    {
        public int QtyPerPkg => (int)(Qty / NoOfPackage);
    }
}
