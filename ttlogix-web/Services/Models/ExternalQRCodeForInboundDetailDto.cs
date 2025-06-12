namespace TT.Services.Models
{
    public class ExternalQRCodeForInboundDetailDto : QRCodeDto
    {
        [Services.Utilities.RequiredAsJsonError]
        public int LineItem { get; set; }
    }
}
