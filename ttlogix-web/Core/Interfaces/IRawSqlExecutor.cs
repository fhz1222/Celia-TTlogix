using System;
using System.Threading.Tasks;

namespace TT.Core.Interfaces
{
    public interface IRawSqlExecutor
    {
        Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters);
        Task<int> ExecuteSqlInterpolatedAsync(FormattableString sql);
    }

}
