using System;
using System.Collections.Generic;

namespace Persistence.Entities
{
    public partial class TtCustomer
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string BizUnit { get; set; } = null!;
        public string Buname { get; set; } = null!;
        public string PrimaryAddress { get; set; } = null!;
        public string BillingAddress { get; set; } = null!;
        public string ShippingAddress { get; set; } = null!;
        public string Pic1 { get; set; } = null!;
        public string Pic2 { get; set; } = null!;
        public byte Status { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string? CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string? RevisedBy { get; set; }
        public DateTime? RevisedDate { get; set; }
        public string CompanyCode { get; set; } = null!;
        public string Whscode { get; set; } = null!;
    }
}
