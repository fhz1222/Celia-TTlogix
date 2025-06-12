using System.Collections.Generic;
using TT.Services.Services.Utilities;

namespace TT.Services.Models
{
    public class OrderNosDto
    {
        [RequiredAsJsonError]
        public IEnumerable<string> OrderNos { get; set; }
    }
}
