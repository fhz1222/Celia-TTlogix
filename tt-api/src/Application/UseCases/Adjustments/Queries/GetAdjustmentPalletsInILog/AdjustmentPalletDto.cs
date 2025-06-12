namespace Application.UseCases.Adjustments.Queries.GetAdjustmentPalletsInILog;

public class AdjustmentPalletDto
{
    public string Pid { get; set; }
    public int OldQty { get; set; }
    public int NewQty { get; set; }
    public bool IsCPart { get; set; }
    public int? CPartBoxQty { get; set; }
}
