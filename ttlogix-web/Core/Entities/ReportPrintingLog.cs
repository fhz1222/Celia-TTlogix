using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TT.Core.Enums;

namespace TT.Core.Entities
{
    [Table("TT_ReportPrintingLog")]
    public class ReportPrintingLog
    {
        [Key]
        public Guid Id { get; set; }
        [MaxLength(15)]
        public string JobNo { get; set; }
        [MaxLength(100)]
        public string ReportName { get; set; }
        public DateTime PrintedDate { get; set; }
        [MaxLength(10)]
        public string PrintedBy { get; set; }
    }
}


