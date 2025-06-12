using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("SunsetExpiredAlert")]
    public class SunsetExpiredAlert
    {
        public string FactoryID { get; set; }
        public string SupplierID { get; set; }
        public int SunsetPeriod { get; set; }
        public DateTime? LastAlertDate { get; set; }
        public string Email { get; set; }
        public string ProductCode { get; set; }
        public string RevisedBy { get; set; }
        public DateTime? RevisedDate { get; set; }
    }

}


