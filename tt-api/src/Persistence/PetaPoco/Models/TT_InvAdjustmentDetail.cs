using PetaPoco;

namespace Persistence.PetaPoco.Models
{
    [TableName("[dbo].[TT_InvAdjustmentDetail]")]
    [PrimaryKey("JobNo", AutoIncrement = false)]
    [ExplicitColumns]
    public partial class TT_InvAdjustmentDetail
    {
        public const string SqlTableName = "TT_InvAdjustmentDetail";
        [Column]
        public byte Act { get; set; }
        [Column]
        public string CreatedBy { get; set; }
        [Column]
        public DateTime CreatedDate { get; set; }
        [Column]
        public string JobNo { get; set; }
        [Column]
        public int LineItem { get; set; }
        [Column]
        public double NewQty { get; set; }
        [Column]
        public double NewQtyPerPkg { get; set; }
        [Column]
        public double OldQty { get; set; }
        [Column]
        public double OldQtyPerPkg { get; set; }
        [Column]
        public string PID { get; set; }
        [Column]
        public string ProductCode { get; set; }
        [Column]
        public string Remark { get; set; }
        [Column]
        public string RevisedBy { get; set; }
        [Column]
        public DateTime? RevisedDate { get; set; }
    }
}
