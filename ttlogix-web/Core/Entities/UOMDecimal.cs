using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_UOMDecimal")]
    public class UOMDecimal
    {
        public string CustomerCode { get; set; }
        public string UOM { get; set; }
        public int DecimalNum { get; set; }
        public byte Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
    }


}
