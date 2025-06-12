namespace TT.Services.Models
{
    public class PrintLocationLabelsDto
    {
        [Services.Utilities.RequiredAsJsonError]
        public CodesCombo[] Codes { get; set; }
        [Services.Utilities.RequiredAsJsonError]
        public string Printer { get; set; }

        [Services.Utilities.RequiredAsJsonError]
        public int Copies { get; set; }
    }
}
