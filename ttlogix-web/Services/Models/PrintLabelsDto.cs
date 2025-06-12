namespace TT.Services.Models
{
    public class PrintLabelsDto
    {
        [Services.Utilities.RequiredAsJsonError]
        public string[] PID { get; set; }
        [Services.Utilities.RequiredAsJsonError]
        public string Type { get; set; }
        [Services.Utilities.RequiredAsJsonError]
        public string Printer { get; set; }

        [Services.Utilities.RequiredAsJsonError]
        public int Copies { get; set; }
    }
}
