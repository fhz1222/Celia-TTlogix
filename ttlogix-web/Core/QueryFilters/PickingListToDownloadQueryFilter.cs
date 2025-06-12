using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TT.Core.QueryFilters
{
    public class PickingListToDownloadQueryFilter
    {
        [Required]
        public IEnumerable<string> JobNos { get; set; }
        public IEnumerable<string> ProductionLineNumbers { get; set; }
        public bool? IsPalletItem { get; set; }
    }

}
