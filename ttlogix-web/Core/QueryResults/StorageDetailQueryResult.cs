using System;

namespace TT.Core.QueryResults
{
    public class StorageDetailQueryResult
    {
        public string InJobNo { get; set; }
        public int LineItem { get; set; }
        public int SeqNo { get; set; }
        public string ProductCode { get; set; }
        public string PID { get; set; }
        public string ExternalPID { get; set; }
        public int? ExternalSystem { get; set; }
        public decimal Qty { get; set; }
        public decimal QtyPerPkg { get; set; }
        public string LocationCode { get; set; }
        public string PutawayBy { get; set; }
        public string PutawayByName { get; set; }
        public DateTime? PutawayDate { get; set; }
        public string GroupID {get; set;}


    }
}