namespace Application.UseCases.Adjustments.Commands.UpdateAdjustmentCommand;

/// <summary>
/// This class represents adjustment fields for update operation
/// </summary>
public class UpdatedAdjustmentVM
{
    /// <summary>
    /// Adjustment job number; identifies adjustment object
    /// </summary>
    public string JobNo { get; set; } = null!;
    /// <summary>
    /// Reference number; it is mandatory when object is updated
    /// </summary>
    public string? ReferenceNo { get; set; } = null!;
    /// <summary>
    /// Adjustment reason (optional)
    /// </summary>
    public string? Reason { get; set; }
}
