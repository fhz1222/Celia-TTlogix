namespace TT.Services.Models
{
    public class UserUpdateDto : UserDto
    {
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
