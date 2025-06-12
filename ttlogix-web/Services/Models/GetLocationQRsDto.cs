namespace TT.Services.Models
{
    public class GetLocationQRsDto
    {
        [Services.Utilities.RequiredAsJsonError]
        public CodesCombo[] Codes { get; set; }
    }

    public class CodesCombo
    {
        public string Code { get; set; }
        public string WHSCode { get; set; }
    }
}
