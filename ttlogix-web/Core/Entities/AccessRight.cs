using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_AccessRight")]
    public class AccessRight
    {
        public string GroupCode { get; set; }
        public string ModuleCode { get; set; }
        public byte? Status { get; set; }
    }

}


