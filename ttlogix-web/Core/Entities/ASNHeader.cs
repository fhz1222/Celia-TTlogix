using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("ASNHeader")]
    public class ASNHeader
    {
        [Key]
        public string ASNNo { get; set; }
        public string FactoryID { get; set; }
        public string SupplierID { get; set; }
        public string ModeOfTransport { get; set; }
        public int TotalPackages { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal TotalWeight { get; set; }
        public byte DirectToLine { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public byte IsVMI { get; set; }
        public byte NotifyEHP { get; set; }
        public byte NotifyForwarder { get; set; }
        public string Filename { get; set; }
        public string CreatedBy { get; set; }
        public string SupplierInvoiceNumber { get; set; }
        public DateTime? StoreArrivalDateEU { get; set; }
        public string ContainerNoEU { get; set; }
        public string BillOfLadingEU { get; set; }
        public string OrderNoEU { get; set; }
        public DateTime? OrderDateEU { get; set; }
        public string VesselNameEU { get; set; }
        public string OriginPortEU { get; set; }
        public string DestinationPortEU { get; set; }
        public DateTime? ETDEU { get; set; }
        public DateTime? ETAtoPortEU { get; set; }
        public string Remark { get; set; }
        public DateTime? ConfirmedETAEU { get; set; }
        public string AirwayBillNo { get; set; }
        public DateTime? ETAtoWHS { get; set; }
    }
}
