using System;
using TT.Core.Enums;
using TT.Core.Extensions;

namespace TT.Core.QueryResults
{
    public class OutstandingInboundForReportQueryResult
    {
        public string JobNo { get; set; }
        public string ASN { get; set; }
        public DateTime CreatedDate { get; set; }
        public InboundStatus Status { get; set; }
        public string InboundType => string.IsNullOrEmpty(ASN) ? "MANUAL" : "ASN";
        public string StatusString => Status.GetEnumDescription();
    }
}