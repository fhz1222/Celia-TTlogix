using PetaPoco;

namespace Persistence.PetaPoco.Models
{
    [TableName("[dbo].[TT_QuarantineLog]")]
    [ExplicitColumns]
    public partial class TT_QuarantineLog
    {
        public const string SqlTableName = "TT_QuarantineLog";
        [Column]
        public string JobNo { get; set; } = null!;
        [Column]
        public int LineItem { get; set; }
        [Column]
        public string PID { get; set; } = null!;
        [Column]
        public byte Act { get; set; }
        [Column]
        public byte Flag { get; set; }
        [Column]
        public string Remark { get; set; } = null!;
        [Column]
        public string CreatedBy { get; set; } = null!;
        [Column]
        public DateTime CreatedDate { get; set; }
    }
}
