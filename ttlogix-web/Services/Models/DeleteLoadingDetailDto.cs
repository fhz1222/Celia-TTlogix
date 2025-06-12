using System.Collections.Generic;

namespace TT.Services.Models
{
    public class DeleteLoadingDetailDto
    {
        [Services.Utilities.RequiredAsJsonError]
        public string JobNo { get; set; }
        [Services.Utilities.RequiredAsJsonError]
        public IEnumerable<string> OrderNos { get; set; }
    }
}
