using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_PackageType")]
    public class PackageType
    {
        [Key]
        public string Code { get; set; }
        public string Name { get; set; }
        public byte Type { get; set; }
        public byte Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
    }

}


