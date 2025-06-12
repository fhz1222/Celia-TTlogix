using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_PIDCode")]
    public class PIDCode
    {
        [Key]
        public string PIDNo { get; set; }
        public DateTime CreatedDate { get; set; }
    }

}


