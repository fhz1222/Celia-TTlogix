namespace TT.Services.Models
{
    public class AllocationDto
    {
        [Services.Utilities.RequiredAsJsonError]
        public string JobNo { get; set; }
        [Services.Utilities.RequiredAsJsonError]
        public int LineItem { get; set; }
    }
}
