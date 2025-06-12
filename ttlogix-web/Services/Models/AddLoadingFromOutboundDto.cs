using System.Collections.Generic;

namespace TT.Services.Models
{
    public class AddLoadingFromOutboundDto
    {
        public LoadingAddDto Loading { get; set; }
        public IEnumerable<string> OutJobNos { get; set; }
    }
}
