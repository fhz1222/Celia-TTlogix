namespace Persistence.Entities;

public partial class EKanbanHeader
{
    public string OrderNo { get; set; } = null!;
    public string FactoryId { get; set; } = null!;
    public string RunNo { get; set; } = null!;
    public DateTime IssuedDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ConfirmationDate { get; set; }
    public string Instructions { get; set; } = null!;
    /// <summary>
    /// 0=New, 1=Imported, 2=InTransit, 3=Data sent, 4=Completed
    /// </summary>
    public byte Status { get; set; }
    public string OutJobNo { get; set; } = null!;
    public DateTime? Eta { get; set; }
    public string? BlanketOrderNo { get; set; }
    public string? RefNo { get; set; }
}
