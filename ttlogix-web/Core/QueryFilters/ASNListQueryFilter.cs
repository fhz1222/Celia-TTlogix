namespace TT.Core.QueryFilters
{
    public class ASNListQueryFilter : QueryFilterBase
    {
        public string WHSCode { get; set; }
        public string ASNNo { get; set; }
        public string ContainerNo { get; set; }
        public string FactoryID { get; set; }
        public string SupplierID { get; set; }
        public string SupplierName { get; set; }
        public bool? IsVMI { get; set; }
    }
}
