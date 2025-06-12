namespace TT.Services.Models
{
    public class QRCodeDto
    {
        [Services.Utilities.RequiredAsJsonError]
        public string Code { get; set; }
        [Services.Utilities.RequiredAsJsonError]
        public string Name { get; set; }
    }
}
