using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_PalletType")]
    public class PalletType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }


}
