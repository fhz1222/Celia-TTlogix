using Application.Common.Models;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Presentation.Utilities.Interfaces;

namespace Application.Services.LocksService;

public class LocksService : ILocksService
{
    public LocksService(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task DeleteAllAccessLocks()
        => throw new NotImplementedException();

    public async Task<AccessLockDto?> GetAccessLock(string jobNo)
    {
        var entity = await repository.Utils.GetAccessLockAsync(jobNo);
        if (entity == null)
        {
            return null;
        }

        return IsLockValid(entity)
            ? new AccessLockDto
            {
                ClientId = entity.ComputerName,
                UserCode = entity.UserCode
            } : null;
    }

    private bool IsLockValid(AccessLock accessLock)
        => accessLock != null && (!accessLock.Timeout.HasValue || accessLock.LockedTime.Value.AddSeconds(accessLock.Timeout.Value) >= DateTime.Now);

    public async Task<bool> TryCreateOrUpdateAccessLock(string jobNo, string clientID, string moduleName, string userCode)
        => throw new NotImplementedException();

    public async Task<bool> TryDeleteAccessLock(string jobNo, string clientId)
        => throw new NotImplementedException();

    public Task DeletedTimeoutedLocks()
        => throw new NotImplementedException();

    private readonly IRepository repository;
}
