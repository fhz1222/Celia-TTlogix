namespace TT.Core.QueryResults;

public class OutboundOrderSummaryQueryResult
{
    public string SupplierID { get; set; }
    public string ProductCode { get; set; }
    public decimal OrderQty { get; set; }
    public decimal OutboundQty { get; set; }
}
