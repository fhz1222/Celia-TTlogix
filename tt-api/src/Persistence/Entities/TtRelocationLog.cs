namespace Persistence.Entities
{
    public partial class TtRelocationLog
    {
        public string PID { get; set; } = null!;
        public string ExternalPID { get; set; } = null!;
        public string OldWHSCode { get; set; } = null!;
        public string OldLocationCode { get; set; } = null!;
        public string NewWHSCode { get; set; } = null!;
        public string NewLocationCode { get; set; } = null!;
        public int ScannerType { get; set; }
        public string RelocatedBy { get; set; } = null!;
        public DateTime RelocatedDate { get; set; }
    }
}