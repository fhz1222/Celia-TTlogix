using System;
using System.Threading.Tasks;

namespace TT.Services.Interfaces
{
    public interface ILocker
    {
        Task<T> WithLockAsync<T>(Func<Task<T>> action);
    }
}
