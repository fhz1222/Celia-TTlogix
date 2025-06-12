namespace Persistence.Entities;

public partial class TtUnloadingPoint
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string CustomerCode { get; set; } = null!;
}
