using Newtonsoft.Json.Linq;
using ServiceResult;
using System.Collections.Generic;
using System.IO;
using TT.Services.Models;

namespace TT.Services.Interfaces
{
    public interface IReportService
    {
        Stream GenerateReport(string fileName, string whsCode, string title, string formula = null, string subreportFormula = null,
            JObject parameters = null);
        Result<Stream> EDTToCSV(IEnumerable<EDTDataDto> data);
    }
}
