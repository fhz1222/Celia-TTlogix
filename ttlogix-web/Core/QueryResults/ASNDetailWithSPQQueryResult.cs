using TT.Core.Entities;

namespace TT.Core.QueryResults
{
    public class ASNDetailWithSPQQueryResult
    {
        public ASNDetail ASNDetail { get; set; }
        public string ProductCode { get; set; }
        public decimal SPQ { get; set; }
        public string PackageType { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal NetWeight { get; set; }
        public decimal GrossWeight { get; set; }
    }
}