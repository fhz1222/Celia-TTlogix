namespace Application.UseCases.StorageDetails;

public class StorageDetailItemWithPartInfoDto
{
    public string PID { get; set; }
    public string ProductCode { get; set; }
    public string SupplierID { get; set; }
    public decimal Qty { get; set; }
    public DateTime InboundDate { get; set; }
    public byte Ownership { get; set; }
    public string OwnershipString => Ownership == 1  ? "EHP" : "Supplier";
    public string WHSCode { get; set; }
    public int DecimalNum { get; set; }
    public string LocationCode { get; set; }
    public string ExternalPID { get; set; }
    public string RefNo { get; set; }
    public decimal SPQ { get; set; }
    public string GroupID { get; set; }

}