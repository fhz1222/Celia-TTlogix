namespace TT.Services.Models
{
    public class EStockTransferPartsStatusDto
    {
        public string OrderNo { get; set; }
        public string ProductCode { get; set; }
        public string SupplierID { get; set; }
        public decimal QtyOrdered { get; set; }
        public int PkgOrdered { get; set; }
        public decimal OnHandQty { get; set; }
        public int OnHandPkg { get; set; }
        public decimal AllocatedQty { get; set; }
        public int AllocatedPkg { get; set; }
        public decimal QuarantineQty { get; set; }
        public int QuarantinePkg { get; set; }
        public decimal StandbyQty { get; set; }
        public int StandbyPkg { get; set; }
        public decimal StockMaintenanceQty { get; set; }
        public int StockMaintenancePkg { get; set; }
        public decimal AvailableQty { get; set; }
        public int AvailablePkg { get; set; }
        public string Status { get; set; }
    }
}