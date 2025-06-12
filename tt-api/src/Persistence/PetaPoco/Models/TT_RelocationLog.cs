using PetaPoco;

namespace Persistence.PetaPoco.Models
{
    [TableName("[dbo].[TT_RelocationLog]")]
    [ExplicitColumns]
    public partial class TT_RelocationLog
    {
        public const string SqlTableName = "TT_RelocationLog";
        [Column]
        public string PID { get; set; } = null!;
        [Column]
        public string ExternalPID { get; set; } = null!;
        [Column]
        public string OldWHSCode { get; set; } = null!;
        [Column]
        public string OldLocationCode { get; set; } = null!;
        [Column]
        public string NewWHSCode { get; set; } = null!;
        [Column]
        public string NewLocationCode { get; set; } = null!;
        [Column]
        public int ScannerType { get; set; }
        [Column]
        public string RelocatedBy { get; set; } = null!;
        [Column]
        public DateTime RelocatedDate { get; set; }
    }
}
