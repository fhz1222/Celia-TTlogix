using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_OutboundQRCode")]
    public class OutboundQRCode
    {
        [Key]
        public string JobNo { get; set; }
        public Byte[] PickingList { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}


