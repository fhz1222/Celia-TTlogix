using System;

namespace TT.Services.Models
{
    public class EDTDataDto
    {
        public string JobNo { get; set; }
        public DateTime OutboundDate { get; set; }
        public string FactoryName { get; set; }
        public string CustomerCode { get; set; }
        public string SupplierID { get; set; }
        public string CountryCode { get; set; }
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public string IRNo { get; set; }
        public string UOM { get; set; }
        public decimal Qty { get; set; }
        public int PIDCount { get; set; }
        public string InternalWHS { get; set; }
    }
}
