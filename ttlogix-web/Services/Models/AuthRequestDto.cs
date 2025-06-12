using System.ComponentModel.DataAnnotations;

namespace TT.Services.Models
{
    public class AuthRequestDto
    {
        [Required(ErrorMessage = "PasswordIsRequired")]
        public string Password { get; set; }
        [Required(ErrorMessage = "UsernameIsRequired")]
        public string Username { get; set; }
        public string Warehouse { get; set; }
    }
}
