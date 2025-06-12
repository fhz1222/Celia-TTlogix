using PetaPoco;

namespace Persistence.PetaPoco.Models
{
    [TableName(SqlTableName)]
    [PrimaryKey("JobNo", AutoIncrement = false)]
    [ExplicitColumns]
    public class TT_InvAdjustmentMaster
    {
        public const string SqlTableName = "TT_InvAdjustmentMaster";
        [Column]
        public string? CancelledBy { get; set; }

        [Column]
        public DateTime? CancelledDate { get; set; }

        [Column]
        public string? ConfirmedBy { get; set; }

        [Column]
        public DateTime? ConfirmedDate { get; set; }

        [Column]
        public string? CreatedBy { get; set; }

        [Column]
        public DateTime CreatedDate { get; set; }
        [Column]
        public string CustomerCode { get; set; } = null!;

        [Column]
        public string JobNo { get; set; } = null!;

        [Column]
        public byte JobType { get; set; }

        [Column]
        public string? Reason { get; set; }

        [Column]
        public string? RefNo { get; set; }
        
        [Column]
        public string? RevisedBy { get; set; }

        [Column]
        public DateTime? RevisedDate { get; set; }


        [Column]
        public byte Status { get; set; }

        [Column]
        public string WHSCode { get; set; } = null!;

    }
}
