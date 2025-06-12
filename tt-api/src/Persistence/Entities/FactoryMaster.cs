namespace Persistence.Entities;

public class FactoryMaster
{
    public string FactoryId { get; set; } = null!;
    public string FactoryName { get; set; } = null!;
    public bool HasActiveIntegration { get; set; }
    public bool HasSap { get; set; }
}
