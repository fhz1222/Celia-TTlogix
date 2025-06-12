using System;
using TT.Core.Enums;

namespace TT.Services.Models
{
    public class STFStorageDetailWithPartInfoDto
    {
        public string PID { get; set; }
        public string ProductCode { get; set; }
        public string SupplierID { get; set; }
        public decimal Qty { get; set; }
        public DateTime InboundDate { get; set; }
        public Ownership Ownership { get; set; }
        public string WHSCode { get; set; }
        public double DaysInStock { get; set; }
        public string LocationCode { get; set; }
    }
}
