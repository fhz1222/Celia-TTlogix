using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("EOrders")]
    public class EOrder
    {
        public string VendorID { get; set; }
        public string EHPFilledInDate { get; set; }
        public string PartNo { get; set; }
        public string Description { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string SupplierPartNumber { get; set; }
        public string StoreDropPoint { get; set; }
        public string Building { get; set; }
        public string CardSerial { get; set; }
        public string OrderQuantity { get; set; }
        public string UnitOfMeasure { get; set; }
        public string BackOrder { get; set; }
        public string FactoryID { get; set; }
        public string OrderRunNumber { get; set; }
        public string PullOrderDate { get; set; }
        public string PullOrderTime { get; set; }
        public string Status { get; set; }
    }
}
