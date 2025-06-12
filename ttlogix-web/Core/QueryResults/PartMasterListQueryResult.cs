using TT.Core.Enums;

namespace TT.Core.QueryResults
{
    public class PartMasterListQueryResult
    {
        public string CustomerCode { get; set; }
        public string SupplierID { get; set; }
        public string ProductCode1 { get; set; }
        public string ProductCode2 { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        public decimal SPQ { get; set; }
        public ValueStatus Status { get; set; }
        public bool IsDefected { get; set; }

        public string UOMName { get; set; }
        public string iLogReadinessStatus { get; set; }
    }
}