using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("ItemMaster")]
    public class ItemMaster
    {
        public string FactoryID { get; set; }
        public string SupplierID { get; set; }
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        public int PeakUsage { get; set; }
        [Column(TypeName = "decimal(9,3)")]
        public decimal? Width { get; set; }
        [Column(TypeName = "decimal(9,3)")]
        public decimal? Depth { get; set; }
        [Column(TypeName = "decimal(9,3)")]
        public decimal? Height { get; set; }
        public int KanbanQty { get; set; }
        public string Obsolete { get; set; } = string.Empty;
        public long Version { get; set; }
        public int CommitFinishedGoods { get; set; }
        public int CommitWIP { get; set; }
        public int CommitRawMaterials { get; set; }
    }

}


