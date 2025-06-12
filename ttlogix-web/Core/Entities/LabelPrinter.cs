using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_LabelPrinter")]
    public class LabelPrinter
    {
        [Key]
        public string IP { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string RevisedBy { get; set; }
        public DateTime? RevisedDate { get; set; }
        public int Type { get; set; }
    }
}


