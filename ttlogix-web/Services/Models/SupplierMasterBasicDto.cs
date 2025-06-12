namespace TT.Services.Models
{
    public class SupplierMasterBasicDto
    {
        public string FactoryID { get; set; }
        public string SupplierID { get; set; }
        public string CompanyName { get; set; }
        public string SupplyParadigm { get; set; }
        public bool IsVMI => SupplyParadigm?.EndsWith('V') == true;
    }
}
