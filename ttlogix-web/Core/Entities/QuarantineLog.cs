using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_QuarantineLog")]
    public class QuarantineLog
    {
        public string JobNo { get; set; }
        public int LineItem { get; set; }
        public string PID { get; set; }
        public byte Act { get; set; }
        public byte Flag { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }

}


