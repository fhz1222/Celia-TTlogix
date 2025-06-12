using System.Linq;
using TT.Core.QueryResults;

namespace TT.Services.Models
{
    public class OutboundListItemDto : OutboundListQueryResult
    {
        public string SupplierName => SupplierNameList != null ? string.Join(", ", SupplierNameList.Distinct()) : string.Empty;
    }
}
