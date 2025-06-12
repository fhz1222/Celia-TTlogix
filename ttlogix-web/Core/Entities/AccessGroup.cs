using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TT.Core.Enums;

namespace TT.Core.Entities
{
    [Table("TT_AccessGroup")]
    public class AccessGroup
    {
        [Key]
        public string Code { get; set; }
        public string Description { get; set; }
        public ValueStatus Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string RevisedBy { get; set; }
        public DateTime? RevisedDate { get; set; }
    }
}


