using PetaPoco;

namespace Persistence.PetaPoco.Models
{
    [TableName("[dbo].[TT_Decant]")]
    [PrimaryKey("JobNo", AutoIncrement = false)]
    [ExplicitColumns]
    public partial class TT_Decant
    {
        public const string SqlTableName = "TT_Decant";
        [Column]
        public string CancelledBy { get; set; } = null!;
        [Column]
        public DateTime? CancelledDate { get; set; }
        [Column]
        public string ConfirmedBy { get; set; } = null!;
        [Column]
        public DateTime? ConfirmedDate { get; set; }
        [Column]
        public string CreatedBy { get; set; } = null!;
        [Column]
        public DateTime CreatedDate { get; set; }
        [Column]
        public string CustomerCode { get; set; } = null!;
        [Column]
        public string JobNo { get; set; } = null!;
        [Column]
        public string RefNo { get; set; } = null!;
        [Column]
        public string Remark { get; set; } = null!;
        [Column]
        public string RevisedBy { get; set; } = null!;
        [Column]
        public DateTime? RevisedDate { get; set; }
        [Column]
        public byte Status { get; set; }
        [Column]
        public string WHSCode { get; set; } = null!;
    }
}
