using System.Collections.Generic;
using TT.Core.Entities;

namespace TT.Services.Models
{
    public class UnloadingPointChoiceDto
    {
        public int? DefaultId { get; set; }
        public IEnumerable<UnloadingPoint> Options { get; set; }
    }
}
