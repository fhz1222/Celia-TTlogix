using System.Collections.Generic;

namespace TT.Services.Models
{
    public class RemovePIDsDto
    {
        [Services.Utilities.RequiredAsJsonError]
        public string JobNo { get; set; }
        [Services.Utilities.RequiredAsJsonError]
        public int LineItem { get; set; }
        public IEnumerable<string> PIDs { get; set; }
        public bool RemoveAll { get; set; }
    }
}
