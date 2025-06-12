using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_OutboundReleaseBondedLog")]
    public class OutboundReleaseBondedLog
    {
        public string JobNo { get; set; }
        public string PID { get; set; }
        public string ReleasedBy { get; set; }
        public DateTime? ReleasedDate { get; set; }
    }
}


