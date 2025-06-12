using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("FactoryMaster")]
    public class FactoryMaster
    {
        [Key]
        public string FactoryID { get; set; }
        public string FactoryName { get; set; }
    }

}


