using System.Text.Json.Serialization;
using TT.Core.Enums;

namespace TT.Core.QueryFilters
{
    public class UserListQueryFilter : QueryFilterBase
    {
        public string Code { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string GroupCode { get; set; }
        public string WHSCode { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ValueStatus? Status { get; set; }
    }

}
