namespace TT.Services.Models
{
    public class UndoAllocationDto
    {
        [Services.Utilities.RequiredAsJsonError]
        public string JobNo { get; set; }
        [Services.Utilities.RequiredAsJsonError]
        public int LineItem { get; set; }
        public int? SeqNo { get; set; }
        public string PID { get; set; }
    }
}
