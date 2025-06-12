using System.Collections.Generic;

namespace TT.Core.QueryFilters
{
    public class QueryFilterBase
    {
        public string OrderBy { get; set; }
        public bool Desc { get; set; }
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 25;
    }
}
