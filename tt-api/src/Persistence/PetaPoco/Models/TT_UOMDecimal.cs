using PetaPoco;

namespace Persistence.PetaPoco.Models
{
    [TableName("[dbo].[TT_UOMDecimal]")]
    [PrimaryKey("Code", AutoIncrement = false)]
    [ExplicitColumns]
    public partial class TT_UOMDecimal
    {
        public const string SqlTableName = "TT_UOMDecimal";
        [Column]
        public string CustomerCode{ get; set; } = null!;
        [Column]
        public string UOM { get; set; } = null!;
        [Column]
        public int DecimalNum { get; set; }
        [Column]
        public byte Status { get; set; }
        [Column]
        public string CreatedBy { get; set; } = null!;
        [Column]
        public DateTime CreatedDate { get; set; }
        [Column]
        public string? CancelledBy { get; set; } = null!;
        [Column]
        public DateTime? CancelledDate { get; set; }
    }
}
