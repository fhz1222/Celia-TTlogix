using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TT.Core.Interfaces;

namespace TT.DB
{
    public class RawSqlExecutor : IRawSqlExecutor
    {
        public RawSqlExecutor(Context dbContext) => this.dbContext = dbContext;

        public async Task<int> ExecuteSqlInterpolatedAsync(FormattableString sql)
        {
            return await dbContext.Database.ExecuteSqlInterpolatedAsync(sql);
        }

        public async Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters)
        {
            return await dbContext.Database.ExecuteSqlRawAsync(sql, parameters);
        }
        private readonly Context dbContext;
    }


}
