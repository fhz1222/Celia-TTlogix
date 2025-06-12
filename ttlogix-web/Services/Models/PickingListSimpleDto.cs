using System;
using TT.Core.Enums;
using TT.Core.QueryResults;

namespace TT.Services.Models
{
    public class PickingListSimpleDto : PickingListSimpleQueryResult
    {
        public string OwnershipString => Enum.IsDefined(typeof(Ownership), Ownership) ? ((Ownership)Ownership).ToString() : "N/A";
    }
}
