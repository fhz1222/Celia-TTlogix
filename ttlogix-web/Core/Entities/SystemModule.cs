using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_SystemModule")]
    public class SystemModule
    {
        [Key]
        public string Code { get; set; }
        public string ParentCode { get; set; }
        public string ModuleName { get; set; }
        public string ShortName { get; set; }
    }

}


