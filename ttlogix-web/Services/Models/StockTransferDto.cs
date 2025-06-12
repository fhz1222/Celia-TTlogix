using System;
using System.Text.Json.Serialization;
using TT.Core.Enums;
using TT.Services.Services.Utilities;

namespace TT.Services.Models
{
    public class StockTransferDto
    {
        [RequiredAsJsonError]
        public string JobNo { get; set; }
        [RequiredAsJsonError]
        public string CustomerCode { get; set; }
        [RequiredAsJsonError]
        public string WHSCode { get; set; }
        public string RefNo { get; set; }
        public string Remark { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public StockTransferType TransferType { get; set; }
        public string TransferTypeString
        {
            get => TransferType.ToString();
            set => TransferType = Enum.Parse<StockTransferType>(value);
        }
        public StockTransferStatus Status { get; set; }
        public string StatusString
        {
            get => Status.ToString();
            set => Status = Enum.Parse<StockTransferStatus>(value);
        }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string RevisedBy { get; set; }
        public DateTime? RevisedDate { get; set; }
        public string ConfirmedBy { get; set; }
        public DateTime? ConfirmedDate { get; set; }
        public string CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string CommInvNo { get; set; }
        public DateTime? CommInvDate { get; set; }
        public bool? DESADV { get; set; }
        public string Currency { get; set; }
        public bool IsMixedCurrency { get; set; }
        public decimal OutboundTotalValue { get; set; }
    }
}
