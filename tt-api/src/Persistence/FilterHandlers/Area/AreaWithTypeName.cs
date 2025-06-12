using Domain.ValueObjects;

namespace Persistence.FilterHandlers.Area;

class AreaWithTypeName
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public byte Status { get; set; }
}
