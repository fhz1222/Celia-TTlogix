using PetaPoco;

namespace Persistence.PetaPoco.Models
{
    [TableName(SqlTableName)]
    [PrimaryKey("Code", AutoIncrement = false)]
    [ExplicitColumns]
    public class TT_Customer
    {
        public const string SqlTableName = "TT_Customer";
        [Column]
        public string BillingAddress { get; set; } = null!;
        [Column]
        public string BizUnit { get; set; } = null!;
        [Column]
        public string BUName { get; set; } = null!;
        [Column]
        public string CancelledBy { get; set; } = null!;
        [Column]
        public DateTime? CancelledDate { get; set; }
        [Column]
        public string Code { get; set; } = null!;
        [Column]
        public string CompanyCode { get; set; } = null!;
        [Column]
        public string CreatedBy { get; set; } = null!;
        [Column]
        public DateTime CreatedDate { get; set; }
        [Column]
        public string Name { get; set; } = null!;
        [Column]
        public string PIC1 { get; set; } = null!;
        [Column]
        public string PIC2 { get; set; } = null!;
        [Column]
        public string PrimaryAddress { get; set; } = null!;
        [Column]
        public string RevisedBy { get; set; } = null!;
        [Column]
        public DateTime? RevisedDate { get; set; } = null!;
        [Column]
        public string ShippingAddress { get; set; } = null!;
        [Column]
        public byte Status { get; set; }
        [Column]
        public string WHSCode { get; set; } = null!;
    }
}
