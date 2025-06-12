using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_ErrorLog")]
    public class ErrorLog
    {
        public string JobNo { get; set; }
        public string Method { get; set; }
        public string ErrorMessage { get; set; }
        public byte Notify { get; set; }
        public DateTime CreatedDate { get; set; }
    }

}


