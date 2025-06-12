using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_JobCode")]
    public class JobCode
    {
        [Key]
        public int Code { get; set; }
        public string Prefix { get; set; }
        public string Name { get; set; }
        public byte Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
    }
}
