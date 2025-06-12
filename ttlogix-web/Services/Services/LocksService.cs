using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TT.Core.Entities;
using TT.Core.Interfaces;
using TT.Services.Interfaces;
using TT.Services.Models;

namespace TT.Services.Services
{
    public class LocksService : ServiceBase<LocksService>, ILocksService
    {
        static readonly int lockTimeout = 60;

        public LocksService(ITTLogixRepository repository, ILocker locker, ILogger<LocksService> logger) : base(locker, logger)
        {
            this.repository = repository;
        }

        public async Task DeleteAllAccessLocks()
        {
            await repository.DeleteAllAccessLocksAsync();
        }

        public async Task<AccessLockDto> GetAccessLock(string jobNo)
        {
            var entity = await repository.GetAccessLockAsync(jobNo);
            return IsLockValid(entity)
                ? new AccessLockDto
                {
                    ClientId = entity.ComputerName,
                    UserCode = entity.UserCode
                } : null;
        }

        private bool IsLockValid(AccessLock accessLock)
        {
            return accessLock != null && (!accessLock.Timeout.HasValue || accessLock.LockedTime.Value.AddSeconds(accessLock.Timeout.Value) >= DateTime.Now);
        }

        public async Task<bool> TryCreateOrUpdateAccessLock(string jobNo, string clientID, string moduleName, string userCode)
        {
            var existing = await repository.GetAccessLockAsync(jobNo);
            //not exists => create
            if (existing == null)
            {
                var model = new AccessLock
                {
                    UserCode = userCode,
                    JobNo = jobNo,
                    ComputerName = clientID,
                    LockedTime = DateTime.Now,
                    ModuleName = moduleName,
                    Timeout = lockTimeout
                };
                await repository.AddAccessLockAsync(model);
            }
            //exists, is valid and not mine => discard
            else if (IsLockValid(existing) && existing.ComputerName != clientID) return false;
            //exists, is either invalid or mine => update
            else
            {
                existing.LockedTime = DateTime.Now;
                existing.UserCode = userCode;
                existing.ModuleName = moduleName;
                existing.ComputerName = clientID;
                existing.Timeout = lockTimeout;
                await repository.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> TryDeleteAccessLock(string jobNo, string clientId)
        {
            var entity = await repository.GetAccessLockAsync(jobNo);
            if (entity != null && entity.ComputerName == clientId)
            {
                await repository.DeleteAccessLockAsync(entity);
                return true;
            }
            return false;
        }

        public Task DeletedTimeoutedLocks() => repository.DeleteTimeoutedLocksAsync();

        private readonly ITTLogixRepository repository;
    }
}
