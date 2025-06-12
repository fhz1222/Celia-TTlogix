using System;

namespace TT.Core.QueryResults
{
    public class PickingListSimpleQueryResult
    {
        public string JobNo { get; set; }
        public int LineItem { get; set; }
        public int SeqNo { get; set; }
        public string ProductCode { get; set; }
        public string SupplierID { get; set; }
        public decimal Qty { get; set; }
        public string PID { get; set; }
        public string WHSCode { get; set; }
        public string LocationCode { get; set; }
        //public string PackageID { get; set; }
        public DateTime InboundDate { get; set; }
        public string InboundJobNo { get; set; }

        //public DateTime? ControlDate { get; set; }
        //public byte? ControlCodeType { get; set; }
        //public string ControlCode { get; set; }
        //public string ControlCodeValue { get; set; }
        //public string DropPoint { get; set; }
        //public string ProductionLine { get; set; }
        //public int? Version { get; set; }
        //public string DownloadBy { get; set; }
        //public DateTime? DownloadDate { get; set; }
        public string PickedBy { get; set; }
        public DateTime? PickedDate { get; set; }
        //public string PackedBy { get; set; }
        //public DateTime? PackedDate { get; set; }


        public string ExternalPID { get; set; }
        public int Ownership { get; set; }

        public decimal DecimalNum { get; set; }
        public string InJobNo { get; set; }

        public decimal? PIDValue { get; set; }
        public string Currency { get; set; }
        public decimal Price { get; set; }
        public string CustomerCode { get; set; }

    }

}