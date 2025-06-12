using System;

namespace TT.Services.Models
{
    public class GroupLabelDto
    {
        [Services.Utilities.RequiredAsJsonError]
        public QRCodeDto Code { get; set; }
        public decimal Qty { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Gid { get; set; }
        public string Name { get; set; }
        public string WHSCode { get; set; }
    }
}
