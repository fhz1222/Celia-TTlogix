using PetaPoco;

namespace Persistence.PetaPoco.Models
{
    [TableName("[dbo].[TT_Owner]")]
    [PrimaryKey("Code", AutoIncrement = false)]
    [ExplicitColumns]
    public partial class TT_Owner
    {
        public const string SqlTableName = "TT_Owner";
        [Column]
        public string Code { get; set; } = null!;
        [Column] 
        public string CompanyCode { get; set; } = null!;
        [Column] 
        public string Name { get; set; } = null!;
        [Column] 
        public byte[]? Logo { get; set; }
        [Column] 
        public string Site { get; set; } = null!;
        [Column] 
        public string PrimaryAddress { get; set; } = null!;
        [Column] 
        public string BillingAddress { get; set; } = null!;
        [Column] 
        public string ShippingAddress { get; set; } = null!;
        [Column] 
        public string PIC1 { get; set; } = null!;
        [Column] 
        public string PIC2 { get; set; } = null!;
        [Column] 
        public string WorkingDay { get; set; } = null!;
        [Column] 
        public string CreatedBy { get; set; } = null!;
        [Column] 
        public DateTime CreatedDate { get; set; }
        [Column] 
        public string? RevisedBy { get; set; }
        [Column] 
        public DateTime? RevisedDate { get; set; }
    }
}
