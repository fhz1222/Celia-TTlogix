using System;

namespace TT.Services.Models
{
    public class ReportPrintedDto
    {
        public string ReportName { get; set; }
        public DateTime PrintedDate { get; set; }
        public string PrintedBy { get; set; }
    }
}
