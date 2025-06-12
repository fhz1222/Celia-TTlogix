using System.Collections.Generic;

namespace TT.Services.Models
{
    public class UndoPickEntryDto
    {
        [Services.Utilities.RequiredAsJsonError]
        public string OutJobNo { get; set; }
        [Services.Utilities.RequiredAsJsonError]
        public IEnumerable<string> PIDs { get; set; }
    }
}
