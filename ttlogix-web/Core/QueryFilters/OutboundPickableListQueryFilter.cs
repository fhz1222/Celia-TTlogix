using System.ComponentModel.DataAnnotations;
using TT.Core.Enums;

namespace TT.Core.QueryFilters
{
    public class OutboundPickableListQueryFilter
    {
        [Required]
        public OutboundType OutboundTransType { get; set; }
        [Required]
        public string CustomerCode { get; set; }
        [Required]
        public string SupplierID { get; set; }
        [Required]
        public string WHSCode { get; set; }
        [Required]
        public ValueStatus PartMasterStatus { get; set; }
        public bool OnlyOnHand { get; set; } = false;
    }
}
