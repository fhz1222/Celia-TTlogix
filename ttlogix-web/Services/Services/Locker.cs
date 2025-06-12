using System;
using System.Threading;
using System.Threading.Tasks;
using TT.Services.Interfaces;

namespace TT.Services.Services
{
    public class Locker : ILocker
    {
        public async Task<T> WithLockAsync<T>(Func<Task<T>> action)
        {
            await semaphore.WaitAsync();

            T result = default;
            try
            {
                result = await action();
            }
            finally
            {
                semaphore.Release();
            }

            return result;
        }

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
    }
}
