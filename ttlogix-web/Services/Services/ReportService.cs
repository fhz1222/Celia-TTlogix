using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using ServiceResult;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TT.Services.Interfaces;
using TT.Services.Models;
using TT.Services.Services.Utilities;

namespace TT.Services.Services
{
    public class ReportService : IReportService
    {
        public ReportService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string ApplicationPath
        {
            get
            {
                if (String.IsNullOrEmpty(AppDomain.CurrentDomain.RelativeSearchPath))
                {
                    return AppDomain.CurrentDomain.BaseDirectory;
                }
                else
                {
                    return AppDomain.CurrentDomain.RelativeSearchPath;
                }
            }
        }

        public Stream GenerateReport(string fileName, string whsCode, string title, string formula = null,
            string subreportFormula = null, JObject parameters = null)
        {
            var sb = new DbConnectionStringBuilder();
            sb.ConnectionString = configuration.GetConnectionString("Database");
            sb.TryGetValue("server", out var server);
            sb.TryGetValue("Database", out var database);
            sb.TryGetValue("user id", out var user);
            sb.TryGetValue("password", out var password);

            using (var process = new Process())
            {
                process.StartInfo.FileName = ApplicationPath + "\\ReportWrapper\\ReportWrapper.exe";
                process.StartInfo.ArgumentList.Add(ApplicationPath + "\\Report\\" + fileName);
                process.StartInfo.ArgumentList.Add(whsCode); // correct owner code
                process.StartInfo.ArgumentList.Add((string)server);
                process.StartInfo.ArgumentList.Add((string)database);
                process.StartInfo.ArgumentList.Add((string)user);
                process.StartInfo.ArgumentList.Add((string)password);
                Console.WriteLine(process.StartInfo.FileName);

                process.StartInfo.ArgumentList.Add(title);
                process.StartInfo.ArgumentList.Add(formula ?? string.Empty);
                process.StartInfo.ArgumentList.Add(subreportFormula ?? string.Empty);
                process.StartInfo.ArgumentList.Add((parameters ?? new JObject()).ToString());
                Console.WriteLine(process.StartInfo.ArgumentList.ToString());

                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WorkingDirectory = ApplicationPath + "\\ReportWrapper";

                if (!process.Start())
                {
                    throw new InvalidOperationException();
                }
                MemoryStream s = new MemoryStream();
                process.StandardOutput.BaseStream.CopyTo(s);
                Console.WriteLine("run");

                process.WaitForExit();
                s.Position = 0;
                return s;
            }
        }

        public Result<Stream> EDTToCSV(IEnumerable<EDTDataDto> data)
        {
            StringBuilder sb = new StringBuilder();
            var resultData = new List<string>();

            if (data.Any())
            {
                var firstRow = data.First();
                var pidCount = data.Sum(i => i.PIDCount);

                var header = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}{19}{20}",
                        "EDT",                                                                      // 0 - Fixed Value
                        "GMG200",                                                                   // 1 - Fixed Value
                        firstRow.JobNo.PadRight(10, ' '),          // 2 - Inbound Job No
                        firstRow.OutboundDate.ToString("ddMMyyyy"),     // 3 - Inbound Date
                        firstRow.FactoryName.PadRight(35, ' '),    // 4 - Factory Name
                        string.Empty.PadRight(35, ' '),                                             // 5 - Empty: Address 1
                        string.Empty.PadRight(35, ' '),                                             // 6 - Empty: Address 2
                        string.Empty.PadRight(3, ' '),                                              // 7 - Empty: ISO Code
                        firstRow.CustomerCode.PadRight(10, ' '),   // 8 - Factory ID
                        string.Empty.PadRight(3, ' '),                                              // 9 - Empty: Goods Origin Country
                        string.Empty.PadRight(3, ' '),                                              // 10 - Country
                        string.Empty.PadRight(3, ' '),                                              // 11 - Empty: Currency ISO Code
                        "0000000000.00000",                                                         // 12 - Empty: Total Gross Weight
                        "0000000000.00000",                                                         // 13 - Empty: Total Net Weight
                        pidCount.ToString().PadLeft(6, '0'),         // 14 - Number of Pallet
                        string.Empty.PadRight(5, ' '),                                              // 15 - Empty: Custom WHS Code
                        "S",                                                                        // 16 - Fixed Value
                        string.Empty.PadRight(33, ' '),                                             // 17 - Empty: Previous Custom Registration Number
                        string.Empty.PadRight(16, ' '),                                             // 18 - Empty: Consignee VAT code
                        string.Empty.PadRight(10, ' '),                                             // 19 - Empty: Filler
                        firstRow.InternalWHS.PadRight(5, ' ')     // 20 - Internal WHS
                        );
                resultData.Add(header); // 21 - Next Line
            }
            else
            {
                return new InvalidResult<Stream>(new JsonResultError("NoPIDAvailableForOutboundStockTransfer").ToJson());
            }

            var rowno = 0;
            foreach (var row in data)
            {
                rowno++;
                var dataRow = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}",
                        "EDR",                                                                      // 0 - Fixed Value
                        "GMG200",                                                                   // 1 - Fixed Value
                        rowno.ToString().PadLeft(3, '0'),                                         // 2 - Serial No.
                        row.ProductCode.PadRight(50, ' '),    // 3 - Product Code
                        Regex.Replace(row.Description, @"[^\u0000-\u007F]", "").Replace("\r\n", "").PadRight(50, ' '),    // 4 - Product Description
                        row.IRNo.PadRight(50, ' '),           // 5 - ASN Number
                        "0000000000.00000",                                                         // 6 - Empty: Gross Weight
                        "0000000000.00000",                                                         // 7 - Empty: Net Weight
                        "0000000000.000",                                                           // 8 - Empty: Invoice Value
                        row.UOM.PadRight(3, ' '),             // 9 - UOM
                        row.Qty.ToString().Trim().PadLeft(14, '0'),             // 10 - Qty
                        row.PIDCount.ToString().PadLeft(6, '0'),         // 11 - Number of Packages
                        string.Empty.PadRight(3, ' '),                                              // 12 - Empty: Goods Country of Origin
                        string.Empty.PadRight(12, ' '),                                             // 13 - Empty: HTS Code
                        string.Empty.PadRight(33, ' '),                                             // 14 - Empty: Previous Custom Registration Number
                        string.Empty.PadRight(60, ' '),                                             // 15 - Empty: Commodity Code
                        string.Empty.PadRight(5, ' '),                                              // 16 - Empty: Regime
                        string.Empty.PadRight(40, ' '),                                             // 17 - Empty: Marks and Numbers
                        string.Empty.PadRight(20, ' ')                                             // 18 - Empty: Aspect of Goods
                        );
                resultData.Add(dataRow);
            }
            return new SuccessResult<Stream>(resultData.ToMemoryStream());
        }

        private readonly IConfiguration configuration;
    }
}
