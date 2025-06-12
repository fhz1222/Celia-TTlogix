namespace Presentation.Common;
/// <summary>
/// Contains list of quarantined PIDs to be updated with the quarantine reason text
/// </summary>
public class QuarantineReasonUpdateDto
{
    /// <summary>
    /// Pallet indentifier
    /// </summary>
    public string[] PIDS { get; set; } = null!;
    /// <summary>
    /// Quarantine reason
    /// </summary>
    public string Reason { get; set; } = null!;
}



