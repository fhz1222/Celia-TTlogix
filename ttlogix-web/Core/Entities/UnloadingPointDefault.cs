using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_UnloadingPointDefault")]
    public class UnloadingPointDefault
    {
        public int Id { get; set; }
        public string CustomerCode { get; set; }
        public string SupplierID { get; set; }
        public int DefaultUnloadingPointId { get; set; }
    }


}
