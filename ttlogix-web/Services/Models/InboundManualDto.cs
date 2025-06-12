using System;
using System.Text.Json.Serialization;
using TT.Core.Enums;

namespace TT.Services.Models
{
    public class InboundManualDto
    {
        public string CustomerCode { get; set; }
        public string SupplierID { get; set; }
        public string WHSCode { get; set; }
        public string IRNo { get; set; } = "";
        public string RefNo { get; set; }
        public string Remark { get; set; }
        public DateTime? ETA { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public InboundType? TransType { get; set; }
    }
}
