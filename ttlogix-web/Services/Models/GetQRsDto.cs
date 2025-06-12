namespace TT.Services.Models
{
    public class GetQRsDto
    {
        [Services.Utilities.RequiredAsJsonError]
        public string[] PID { get; set; }
    }
}
