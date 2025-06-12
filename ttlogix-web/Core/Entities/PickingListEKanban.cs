using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_PickingListEKanban")]
    public class PickingListEKanban
    {
        public string JobNo { get; set; } = string.Empty;
        public int LineItem { get; set; }
        public int SeqNo { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public string SerialNo { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
    }
}

