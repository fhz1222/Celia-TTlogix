using System.Collections.Generic;
using TT.Services.Services.Utilities;

namespace TT.Services.Models
{
    public class StockTransferDetailByPIDDto
    {
        [RequiredAsJsonError]
        public string JobNo { get; set; }
        [RequiredAsJsonError]
        public IEnumerable<string> PIDs { get; set; }
    }
}
