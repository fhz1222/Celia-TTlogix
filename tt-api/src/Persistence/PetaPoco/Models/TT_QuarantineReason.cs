using PetaPoco;

namespace Persistence.PetaPoco.Models
{
    [TableName("[dbo].[TT_QuarantineReason]")]
    [ExplicitColumns]
    public partial class TT_QuarantineReason
    {
        public const string SqlTableName = "TT_QuarantineReason";
        [Column]
        public string PID { get; set; } = null!;
        [Column]
        public string Reason{ get; set; } = null!;
        [Column]
        public DateTime CreatedDate { get; set; }
    }
}
