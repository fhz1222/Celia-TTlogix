using System;

namespace TT.Services.Models
{
    public class LoadingAddDto
    {
        public string CustomerCode { get; set; }
        public string WHSCode { get; set; }
        public string RefNo { get; set; }
        public string Remark { get; set; }
        public DateTime? ETD { get; set; }
    }
}
