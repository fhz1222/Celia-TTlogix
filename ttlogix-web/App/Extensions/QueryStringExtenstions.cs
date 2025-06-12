using Microsoft.AspNetCore.Http;
using System.Web;

namespace TT.Extensions
{
    public static class QueryStringExtenstions
    {

        public static QueryString ReplaceOrAdd(this QueryString query, string key, string value)
        {
            var qs = HttpUtility.ParseQueryString(query.ToString());
            qs.Set(key, value);
            return new QueryString("?" + qs.ToString());
        }
    }
}
