using System.Threading.Tasks;

namespace TT.Services.Interfaces
{
    public interface ILoggerService
    {
        Task LogError(string jobNo, string methodName, string message, bool notify);
        void LogInformation(string message);
    }
}
