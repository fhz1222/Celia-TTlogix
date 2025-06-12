namespace TT.Services.Models
{
    public class InboundDetailEntryModifyDto : InboundDetailEntryBaseDto
    {
        public int LineItem { get; set; }
        public string ControlCode3 { get; set; }
    }
}
