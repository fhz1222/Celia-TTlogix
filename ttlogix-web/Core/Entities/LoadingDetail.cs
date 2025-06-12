using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_LoadingDetail")]
    public class LoadingDetail
    {
        [Column(TypeName="varchar(15)")]
        public string JobNo { get; set; }
        [Column(TypeName="varchar(25)")]
        public string OrderNo { get; set; }
        [Column(TypeName="varchar(10)")]
        public string SupplierID { get; set; } = string.Empty;
        public DateTime? ETD { get; set; }
        [Column(TypeName="varchar(10)")]
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        [Column(TypeName="varchar(15)")]
        public string OutJobNo { get; set; }
    }

}
