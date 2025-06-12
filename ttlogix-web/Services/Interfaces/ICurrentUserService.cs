using System.Threading.Tasks;
using TT.Services.Models;

namespace TT.Services.Interfaces
{
    public interface ICurrentUserService
    {
        Task<CurrentUserDto> Authenticate(string username, string password, string warehouse);
        Task<CurrentUserDto> CurrentUser();
    }
}
