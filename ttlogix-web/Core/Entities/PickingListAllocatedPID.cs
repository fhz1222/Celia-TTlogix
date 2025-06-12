using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_PickingList_AllocatedPID")]
    public class PickingListAllocatedPID
    {
        public string JobNo { get; set; }
        public int LineItem { get; set; }
        public int SeqNo { get; set; }
        public string PID { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal Qty { get; set; }
    }

}


