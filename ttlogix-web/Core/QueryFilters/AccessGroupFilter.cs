using TT.Core.Enums;

namespace TT.Core.QueryFilters
{
    public class AccessGroupFilter
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public ValueStatus? Status { get; set; }
        public string OrderBy { get; set; }
        public bool Desc { get; set; }
    }

}
