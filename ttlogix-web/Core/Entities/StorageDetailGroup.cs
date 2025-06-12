using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using TT.Core.Enums;

namespace TT.Core.Entities
{
    [Table("TT_StorageDetailGroup")]
    public class StorageDetailGroup
    {
        [Key]
        public string GroupID { get; set; }
        public DateTime? RepackedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        [NotMapped]
        public StorageGroupStatus Status => (ClosedDate, RepackedDate) switch
        {
            var (cd, rd) when cd.HasValue && !rd.HasValue => StorageGroupStatus.Closed,
            var (cd, rd) when !cd.HasValue && rd.HasValue => StorageGroupStatus.Transformed,
            var (cd, rd) when cd.HasValue && rd.HasValue => cd.Value > rd.Value ? StorageGroupStatus.Closed : StorageGroupStatus.Transformed,
            _ => StorageGroupStatus.InUse
        };
        public DateTime CreatedDate { get; set; }
        public string Name { get; set; }
        public string WHSCode { get; set; }
        public int Quantity { get; set; }
        [NotMapped]
        public IEnumerable<string> InJobNo { get; set; }
    }

}
