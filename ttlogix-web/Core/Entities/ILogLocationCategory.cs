using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_ILogLocationCategory")]
    public class ILogLocationCategory
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}


