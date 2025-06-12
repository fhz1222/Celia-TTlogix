using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Core.Entities
{
    [Table("TT_ELLISPalletType")]
    public class ELLISPalletType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }


}
