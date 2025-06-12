using System;
using System.ComponentModel.DataAnnotations.Schema;
using TT.Core.Enums;

namespace TT.Core.Entities
{
    [Table("TT_Location")]
    public class Location
    {
        public string Code { get; set; }
        public string WHSCode { get; set; }
        public string AreaCode { get; set; }
        public string Name { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal M3 { get; set; }
        public byte Status { get; set; }
        public LocationType Type { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
        public byte? IsPriority { get; set; }
        public int ILogLocationCategoryId { get; set; }
    }

}
