using Application.Common.Models;

namespace Presentation.Utilities.Interfaces;

public interface ILocksService
{
    Task<AccessLockDto?> GetAccessLock(string jobNo);
    Task<bool> TryCreateOrUpdateAccessLock(string jobNo, string clientId, string moduleName, string userCode);
    Task<bool> TryDeleteAccessLock(string jobNo, string clientId);
    Task DeleteAllAccessLocks();
    Task DeletedTimeoutedLocks();
}
