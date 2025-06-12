using TT.Core.Enums;

namespace TT.Core.QueryResults
{
    public class UserListQueryResult
    {
        public string Code { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string WHSCode { get; set; }
        public string GroupCode { get; set; }
        public ValueStatus Status { get; set; }
    }
}