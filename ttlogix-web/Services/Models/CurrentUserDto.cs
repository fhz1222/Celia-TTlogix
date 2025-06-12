using System.Collections.Generic;

namespace TT.Services.Models
{
    public class CurrentUserDto : UserDto
    {
        public string Token { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
