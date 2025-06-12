using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.PetaPoco.Models
{
    [TableName("[dbo].[TT_AccessLock]")]
    [PrimaryKey("JobNo", AutoIncrement = false)]
    [ExplicitColumns]
    public partial class TT_AccessLock
    {
        public const string SqlTableName = "TT_AccessLock";
        [Column]
        public string ComputerName { get; set; }

        [Column]
        public string JobNo { get; set; }
        
        [Column]
        public DateTime? LockedTime { get; set; }
        
        [Column]
        public string ModuleName { get; set; }

        [Column]
        public int? Timeout { get; set; }

        [Column]
        public string UserCode { get; set; }
    }
}
