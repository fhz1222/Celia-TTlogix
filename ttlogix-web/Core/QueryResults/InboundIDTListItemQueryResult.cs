using System;

namespace TT.Core.QueryResults
{
    public class InboundIDTListItemQueryResult
    {
        public string ASNNo { get; set; }
        public DateTime CreatedDate { get; set; }
        public string FactoryID { get; set; }
        public string ModeOfTransport { get; set; }
        public DateTime? StoreArrivalDate { get; set; }
        public string SupplierID { get; set; }
        public string CompanyName { get; set; }
        public int TotalPackages { get; set; }
        public decimal TotalWeight { get; set; }
        public string SupplierInvoiceNumber { get; set; }
        public string ProductCode { get; set; }
        public DateTime? ManufacturedDate { get; set; }
        public string BatchNo { get; set; }
        public int QtyPerOuter { get; set; }
        public int NoOfOuter { get; set; }
        public int TotalQty => QtyPerOuter * NoOfOuter;


    }
}
