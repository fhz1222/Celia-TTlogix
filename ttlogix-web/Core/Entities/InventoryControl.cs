using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_InventoryControl")]
    public class InventoryControl
    {
        [Key]
        public string CustomerCode { get; set; }
        public string PC1Type { get; set; }
        public string PC2Type { get; set; }
        public string PC3Type { get; set; }
        public string PC4Type { get; set; }
        public string CC1Type { get; set; }
        public string CC2Type { get; set; }
        public string CC3Type { get; set; }
        public string CC4Type { get; set; }
        public string CC5Type { get; set; }
        public string CC6Type { get; set; }
        public string RevisedBy { get; set; }
        public DateTime? RevisedDate { get; set; }
        public string SelectControlCode { get; set; }
    }

}
