using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_CycleCountDetail")]
    public class CycleCountDetail
    {
        public string JobNo { get; set; }
        public int LineItem { get; set; }
        public int SeqNo { get; set; }
        public string PID { get; set; }
        public string ProductCode { get; set; }
        public string SupplierID { get; set; }
        public double Qty { get; set; }
        public double CountedQty { get; set; }
        public long NoOfPackage { get; set; }
        public string WHSCode { get; set; }
        public string LocationCode { get; set; }
        public byte Status { get; set; }
        public string CountedBy { get; set; }
        public DateTime? CountedDate { get; set; }
    }

}
