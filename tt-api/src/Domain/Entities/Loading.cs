namespace Domain.Entities;

public class Loading
{
#pragma warning disable CS8618
    public string JobNo { get; set; }
    public string CustomerCode { get; set; }
    public string WarehouseCode { get; set; }
    public DateTime? ETD { get; set; }
    public DateTime? ETA { get; set; }
#pragma warning restore CS8618
}
