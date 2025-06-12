using TT.Core.Enums;

namespace TT.Services.Models
{
    public class AccessGroupDto
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public ValueStatus Status { get; set; }
    }

    public class AccessGroupAddDto : AccessGroupDto
    {
    }
}
