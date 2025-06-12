using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("ASNDetail")]
    public class ASNDetail
    {
        public string ASNNo { get; set; }
        public int LineItem { get; set; }
        public string ProductCode { get; set; }
        public string OrderNo { get; set; }
        public string ContainerNo { get; set; }
        public string ContainerSize { get; set; }
        public string SealNo { get; set; }
        public DateTime? ManufacturedDate { get; set; }
        public string BatchNo { get; set; }
        public int QtyPerOuter { get; set; }
        public int NoOfOuter { get; set; }
        public bool Breakpoint { get; set; }
        public string InJobNo { get; set; }
        public string BillOfLading { get; set; }
        public string MaerskSONo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ExSupplierDate { get; set; }
        public DateTime? ShipDepartureDate { get; set; }
        public DateTime? PortArrivalDate { get; set; }
        public DateTime? StoreArrivalDate { get; set; }
        public string VesselName { get; set; }
        public string VoyageNo { get; set; }
        public string Status { get; set; }
        public string PreImportStatus { get; set; }
        public string PONo { get; set; }
        public string POLineNo { get; set; }
    }
}
