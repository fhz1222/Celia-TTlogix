using System;
using TT.Core.Enums;

namespace TT.Core.QueryResults
{
    public class InboundWithExtendedDataQueryResult
    {
        public string JobNo { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string SupplierID { get; set; }
        public string SupplierName { get; set; }
        public InboundStatus Status { get; set; }
        public InboundType TransType { get; set; }
        public string Remark { get; set; }
        public string WHSCode { get; set; }
        public string IRNo { get; set; }
        public string RefNo { get; set; }
        public DateTime ETA { get; set; }
        public byte Charged { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string RevisedBy { get; set; }
        public DateTime? RevisedDate { get; set; }
        public string CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string PutawayBy { get; set; }
        public DateTime? PutawayDate { get; set; }
        public string Currency { get; set; }
        public string IM4No { get; set; }
        public DateTime? CustomsDeclarationDate { get; set; }
        public string ContainerNo { get; set; }
        public decimal TotalResidualValue { get; set; }
        public decimal TotalASNValue { get; set; }
    }
}