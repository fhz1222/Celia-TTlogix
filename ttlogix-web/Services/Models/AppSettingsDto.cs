namespace TT.Services.Models
{
    public class AppSettingsDto
    {
        public string OwnerCode { get; set; }
        public bool IsTESA => OwnerCode.StartsWith("TESA");
    }
}
