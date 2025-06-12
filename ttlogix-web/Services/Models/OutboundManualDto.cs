using System;
using TT.Core.Enums;
using TT.Services.Models.ModelEnums;

namespace TT.Services.Models
{
    public class OutboundManualDto
    {
        public ManualType ManualType { get; set; }
        public string CustomerCode { get; set; }
        public string WHSCode { get; set; }
        public string OSNo { get; set; }
        public string RefNo { get; set; }
        public DateTime ETD { get; set; }
        public OutboundType TransType { get; set; }
        public string NewWHSCode { get; set; }
        public OutboundStatus Status { get; set; }
        public string Remark { get; set; }
    }
}
