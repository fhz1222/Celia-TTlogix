using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TT.Core.Entities
{
    [Table("TT_Customer")]
    public class Customer
    {
        public string Code { get; set; } = string.Empty;
        public string WHSCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string BizUnit { get; set; } = string.Empty;
        public string BUName { get; set; } = string.Empty;
        public string PrimaryAddress { get; set; } = string.Empty;
        public string BillingAddress { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string PIC1 { get; set; } = string.Empty;
        public string PIC2 { get; set; } = string.Empty;
        public byte Status { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string CancelledBy { get; set; } = string.Empty;
        public DateTime? CancelledDate { get; set; }
        public string RevisedBy { get; set; } = string.Empty;
        public DateTime? RevisedDate { get; set; }
        public string CompanyCode { get; set; } = string.Empty;

    }

}
