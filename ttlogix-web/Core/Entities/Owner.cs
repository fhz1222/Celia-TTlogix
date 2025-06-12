using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_Owner")]
    public class Owner
    {
        [Key]
        public string Code { get; set; }
        public string CompanyCode { get; set; }
        public string Name { get; set; }
        public System.Byte[] Logo { get; set; }
        public string Site { get; set; }
        public string PrimaryAddress { get; set; }
        public string BillingAddress { get; set; }
        public string ShippingAddress { get; set; }
        public string PIC1 { get; set; }
        public string PIC2 { get; set; }
        public string WorkingDay { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string RevisedBy { get; set; }
        public DateTime? RevisedDate { get; set; }
    }
}


