namespace TT.Services.Models
{
    public class CustomerBasicDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string FullName => $"{Code} - {Name}";
    }
}
