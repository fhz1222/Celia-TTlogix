using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("DELFORHeader")]
    public class DelforHeader
    {
        public string EDIID { get; set; }
        public string EDISender { get; set; }
        public string EDIRecipient { get; set; }
        public DateTime EDIDate { get; set; }
        public string MessageType { get; set; }
        public string MessageTypeVersion { get; set; }
        public string MessageTypeRelease { get; set; }
        public string DocumentNo { get; set; }
        public string MessageFunction { get; set; }
        public DateTime DocumentDate { get; set; }
        public string SellerQualifier { get; set; }
        public string SellerID { get; set; }
        public string BuyerID { get; set; }
        public int Status { get; set; }
        public string FileName { get; set; }
    }
}
