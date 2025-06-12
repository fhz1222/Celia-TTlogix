namespace Application.UseCases.PalletTransferRequests;

public class PalletTransferRequestDto
{
    public string JobNo { get; set; }
    public string CustomerCode { get; set; }
    public string WhsCode { get; set; }
    public string ProductCode { get; set; }
    public decimal Qty { get; set; }
    public string SupplierID { get; set; }
    public string PID { get; set; }

    public PalletTransferRequestDto(string jobNo, string customerCode, string whsCode, string productCode, decimal qty, string supplierID, string pID)
    {
        JobNo = jobNo;
        CustomerCode = customerCode;
        WhsCode = whsCode;
        ProductCode = productCode;
        Qty = qty;
        SupplierID = supplierID;
        PID = pID;
    }
}