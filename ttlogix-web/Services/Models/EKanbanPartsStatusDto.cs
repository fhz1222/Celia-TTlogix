namespace TT.Services.Models
{
    public class EKanbanPartsStatusDto
    {
        public string OrderNo { get; set; }
        public string ProductCode { get; set; }
        public string SupplierID { get; set; }
        public decimal QtyOrdered { get; set; }
        public decimal PkgOrdered { get; set; }
        public decimal OnHandQty { get; set; }
        public decimal OnHandPkg { get; set; }
        public decimal AllocatedQty { get; set; }
        public decimal AllocatedPkg { get; set; }
        public decimal QuarantineQty { get; set; }
        public decimal QuarantinePkg { get; set; }
        public decimal StandbyQty { get; set; }
        public decimal StandbyPkg { get; set; }
        public decimal StockMaintenanceQty { get; set; }
        public decimal StockMaintenancePkg { get; set; }
        public decimal AvailableQty { get; set; }
        public decimal AvailablePkg { get; set; }
        public decimal ELXOnHandQty { get; set; }
        public decimal ELXOnhandPkg { get; set; }
        public decimal ELXAllocatedQty { get; set; }
        public decimal ELXAllocatedPkg { get; set; }
        public decimal ELXQuarantineQty { get; set; }
        public decimal ELXQuarantinePkg { get; set; }
        public decimal ELXStandByQty { get; set; }
        public decimal ELXStandbyPkg { get; set; }
        public decimal ELXStockMaintenanceQty { get; set; }
        public decimal ELXStockMaintenancePkg { get; set; }
        public decimal ELXAvailableQty { get; set; }
        public decimal ELXAvailablePkg { get; set; }
        public string Status { get; set; }
    }
}