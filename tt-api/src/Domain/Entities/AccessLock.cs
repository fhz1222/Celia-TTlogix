namespace Domain.Entities;

public class AccessLock
{
    public string JobNo { get; set; } = null!;
    public string ComputerName { get; set; } = null!;
    public string UserCode { get; set; } = null!;
    public string ModuleName { get; set; } = null!;
    public DateTime? LockedTime { get; set; }
    public int? Timeout { get; set; }
}
