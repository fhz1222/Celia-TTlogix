using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_UnloadingPoint")]
    public class UnloadingPoint
    {
        public int Id { get; set; }
        public string CustomerCode { get; set; }
        public string Name { get; set; }
    }


}
