namespace Presentation.Common;

public class RelocatePalletDto
{
    public string PID { get; set; } = null!;
    public string NewLocation { get; set; } = null!;
    public DateTime RelocatedOn { get; set; }
    public string AllowedTrgLocationCategory { get; set; }
}



