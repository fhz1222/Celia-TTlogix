using PetaPoco;
using PetaPoco.SqlKata;
using PetaPoco.Utilities;
using SqlKata;

namespace Persistence.PetaPoco.Extensions
{
    public static class PetaPocoExtensions
    {
        // it works only if order by occurs in the query
        public static Page<TRet> PagedFetch<T1, T2, TRet>(this Database db, int pageNo, int itemsPerPage, Func<T1, T2, TRet> cb, Query query)
        {
            SQLParts parts;
            var sqlObj = query.ToSql();
            
            if (!db.Provider.PagingUtility.SplitSQL(query.ToSql().SQL, out parts))
                throw new Exception("Unable to parse SQL statement for paged query");

            // apply paging
            query.ForPage(pageNo, itemsPerPage);
            
            var data = db.Fetch(cb, query.ToSql());
            return GeneratePagedResult(db, data, parts, sqlObj, itemsPerPage, pageNo); 
        }

        public static Page<TRet> PagedFetch<T1, T2, T3, TRet>(this Database db, int pageNo, int itemsPerPage, Func<T1, T2, T3, TRet> cb, Query query)
        {
            SQLParts parts;
            var sqlObj = query.ToSql();

            if (!db.Provider.PagingUtility.SplitSQL(query.ToSql().SQL, out parts))
                throw new Exception("Unable to parse SQL statement for paged query");

            // apply paging
            query.ForPage(pageNo, itemsPerPage);

            var data = db.Fetch(cb, query.ToSql());
            return GeneratePagedResult(db, data, parts, sqlObj, itemsPerPage, pageNo);
        }

        public static Page<TRet> PagedFetch<T1, T2, T3, T4, TRet>(this Database db, int pageNo, int itemsPerPage, Func<T1, T2, T3, T4, TRet> cb, Query query)
        {
            SQLParts parts;
            var sqlObj = query.ToSql();

            if (!db.Provider.PagingUtility.SplitSQL(query.ToSql().SQL, out parts))
                throw new Exception("Unable to parse SQL statement for paged query");

            // apply paging
            query.ForPage(pageNo, itemsPerPage);

            var data = db.Fetch(cb, query.ToSql());
            return GeneratePagedResult(db, data, parts, sqlObj, itemsPerPage, pageNo);
        }

        public static Page<TRet> PagedFetch<T1, T2, T3, T4, T5, TRet>(this Database db, int pageNo, int itemsPerPage, Func<T1, T2, T3, T4, T5, TRet> cb, Query query)
        {
            SQLParts parts;
            var sqlObj = query.ToSql();

            if (!db.Provider.PagingUtility.SplitSQL(query.ToSql().SQL, out parts))
                throw new Exception("Unable to parse SQL statement for paged query");

            // apply paging
            query.ForPage(pageNo, itemsPerPage);

            var data = db.Fetch(cb, query.ToSql());
            return GeneratePagedResult(db, data, parts, sqlObj, itemsPerPage, pageNo);
        }



        private static Page<TRet> GeneratePagedResult<TRet>(Database db, List<TRet> data, SQLParts parts, Sql sqlObj, int itemsPerPage, int pageNo)
        {
            var result = new Page<TRet>
            {
                Items = data,
                TotalItems = db.ExecuteScalar<long>(parts.SqlCount, sqlObj.Arguments),
                CurrentPage = pageNo,
                ItemsPerPage = itemsPerPage
            };

            result.TotalPages = result.TotalItems / itemsPerPage;

            if ((result.TotalItems % itemsPerPage) != 0)
                result.TotalPages++;

            return result;
        }
    }
}
