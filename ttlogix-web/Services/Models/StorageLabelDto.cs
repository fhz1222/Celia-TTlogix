using System;

namespace TT.Services.Models
{
    public class StorageLabelDto
    {
        [Services.Utilities.RequiredAsJsonError]
        public QRCodeDto Code { get; set; }
        public string PartNo { get; set; }
        public decimal Qty { get; set; }
        public string SupplierID { get; set; }
        public string SupplierName { get; set; }
        public DateTime DateRecieved { get; set; }
        public string InboundJobNo { get; set; }
        public string Pid { get; set; }
        public string ControlCode1Header { get; set; }
        public string ControlCode1 { get; set; }
        public string ControlCode2Header { get; set; }
        public string ControlCode2 { get; set; }
        public string ASNNo { get; set; }
        public string CustomerID { get; set; }
        public string Description { get; set; }
    }
}
