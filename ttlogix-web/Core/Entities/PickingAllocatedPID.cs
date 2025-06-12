using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_PickingAllocatedPID")]
    public class PickingAllocatedPID
    {
        public string JobNo { get; set; }
        public int LineItem { get; set; }
        public int SerialNo { get; set; }
        public string PID { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal? AllocatedQty { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal? PickedQty { get; set; }
    }
}


