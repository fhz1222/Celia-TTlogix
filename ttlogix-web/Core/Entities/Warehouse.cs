using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_Warehouse")]
    public class Warehouse
    {
        [Key]
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
        public string PIC { get; set; }
        public string TelNo { get; set; }
        public string FaxNo { get; set; }
        [Column(TypeName = "numeric(18,0)")]
        public decimal Area { get; set; }
        public byte Type { get; set; }
        public byte Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
    }

}


