using TT.Core.Enums;
using TT.Services.Services.Utilities;

namespace TT.Services.Models
{
    public class UserDto
    {
        [RequiredAsJsonError]
        public string Code { get; set; }
        [RequiredAsJsonError]
        public string FirstName { get; set; }
        [RequiredAsJsonError]
        public string LastName { get; set; }
        [RequiredAsJsonError]
        public string WHSCode { get; set; }
        [RequiredAsJsonError]
        public string GroupCode { get; set; }
        public ValueStatus Status { get; set; }
        public string FullName => $"{FirstName}, {LastName}";
    }
}
