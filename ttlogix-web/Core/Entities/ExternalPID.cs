using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_ExternalPID")]
    public class ExternalPID
    {
        [Key]
        public string PID { get; set; }
        [Column("ExternalPID")]
        public string ExternalID { get; set; }
        public byte ExternalSystem { get; set; }
        public string InJobNo { get; set; }
        public int InLineItem { get; set; }
    }

}
