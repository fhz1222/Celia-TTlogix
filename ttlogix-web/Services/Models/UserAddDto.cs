using TT.Services.Services.Utilities;

namespace TT.Services.Models
{
    public class UserAddDto : UserDto
    {
        [RequiredAsJsonError]
        public string Password { get; set; }
        [RequiredAsJsonError]
        public string ConfirmPassword { get; set; }
    }
}
