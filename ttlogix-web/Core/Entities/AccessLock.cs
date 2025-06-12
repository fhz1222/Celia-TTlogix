using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_AccessLock")]
    public class AccessLock
    {
        [Key]
        public string JobNo { get; set; }
        public string ComputerName { get; set; }
        public string UserCode { get; set; }
        public string ModuleName { get; set; }
        public DateTime? LockedTime { get; set; }
        public int? Timeout { get; set; }
    }

}


