using System;
using TT.Core.Entities;
using TT.Core.Enums;

namespace TT.Core.QueryResults
{
    public class StorageDetailExtendedQueryResult
    {
        public StorageDetail StorageDetail { get; set; }
        public string PID { get; set; }
        public string InJobNo { get; set; }
        public int LineItem { get; set; }
        public int SeqNo { get; set; }
        public string ProductCode { get; set; }
        public string CustomerCode { get; set; }
        public decimal Qty { get; set; }
        public decimal AllocatedQty { get; set; }
        public string OutJobNo { get; set; }
        public string LocationCode { get; set; }
        public StorageStatus Status { get; set; }
        public string SupplierID { get; set; }
        public Ownership Ownership { get; set; }
        public LocationType LocationType { get; set; }
        public decimal? PickedAllocatedQty { get; set; }
        public bool IsReturn => LocationCode == Enum.GetName(typeof(ExtSystemLocation), (int)ExtSystemLocation.RETURN);
    }
}