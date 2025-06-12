namespace Persistence.Entities
{
    public class TtUOMDecimal
    {
        public string CustomerCode { get; set; }
        public string UOM { get; set; }
        public int DecimalNum { get; set; }
        public byte Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
    }
}
