using System.Collections.Generic;
using System.IO;
using TT.Core.QueryResults;

namespace TT.Services.Interfaces
{
    public interface IXlsService
    {
        Stream WriteOutstandingInboundsToXls(IEnumerable<OutstandingInboundForReportQueryResult> data);
    }
}
