namespace TT.Services.Models
{
    public class InboundDetailEntryAddDto : InboundDetailEntryBaseDto
    {
        public decimal Qty { get; set; }
        public decimal ImportedQty { get; set; }
        public string PackageType { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal NetWeight { get; set; }
        public decimal GrossWeight { get; set; }
        public string Remark { get; set; }
        public string ControlCode1 { get; set; }
        public string ControlCode2 { get; set; }
        public string ControlCode3 { get; set; }
        public string ControlCode4 { get; set; }
        public string ControlCode5 { get; set; }
        public string ControlCode6 { get; set; }
    }
}
