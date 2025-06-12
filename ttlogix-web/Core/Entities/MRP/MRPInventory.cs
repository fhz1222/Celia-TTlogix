using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT.Core.Entities.MRP
{
    [Table("Inventory")]
    public class MRPInventory
    {
        public string FactoryID { get; set; }
        public string SupplierID { get; set; }
        public string ProductCode { get; set; }
        public int OnHandQty { get; set; }
        public int AllocatedQty { get; set; }
        public int OnHoldQty { get; set; }
        public int TransitQty { get; set; }
    }
}
