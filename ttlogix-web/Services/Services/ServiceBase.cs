using Microsoft.Extensions.Logging;
using ServiceResult;
using System;
using System.Threading.Tasks;
using System.Transactions;
using TT.Services.Interfaces;

namespace TT.Services.Services
{
    public abstract class ServiceBase<V> where V : class
    {
        public ServiceBase(ILocker locker, ILogger<V> logger)
        {
            this.locker = locker;
            this.logger = logger;
        }

        public async Task<Result<T>> WithTransactionScopeAndLock<T>(Func<Task<Result<T>>> action)
        {
            return await locker.WithLockAsync<Result<T>>(async () =>
            {
                return await WithTransactionScope<T>(action);
            });
        }

        public async Task<Result<T>> WithTransactionScope<T>(Func<Task<Result<T>>> action)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions()
            {
                Timeout = TimeSpan.FromMinutes(3),
                IsolationLevel = IsolationLevel.RepeatableRead
            }, TransactionScopeAsyncFlowOption.Enabled))
            {
                Result<T> result = await action();
                if (result.ResultType == ResultType.Ok)
                {
                    scope.Complete();
                }
                return result;
            }
        }

        private readonly ILocker locker;
        private readonly ILogger<V> logger;
    }
}
