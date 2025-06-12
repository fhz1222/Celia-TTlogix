using System;
using System.Collections.Generic;
using TT.Core.Enums;

namespace TT.Core.QueryResults
{
    public class InboundListQueryResult
    {
        public string JobNo { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string RefNo { get; set; }
        public string WHSCode { get; set; }
        public string SupplierName { get; set; }
        /// <summary>
        /// IRNo column
        /// </summary>
        public string ASNNumber { get; set; }
        public IEnumerable<string> ContainerNos { get; set; }
        public InboundType TransType { get; set; }
        public DateTime ReceivedDate { get; set; }
        public InboundStatus Status { get; set; }
        public string Remark { get; set; }
    }
}