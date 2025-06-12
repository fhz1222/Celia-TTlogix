namespace TT.Services.Models
{
    public abstract class InboundDetailEntryBaseDto
    {
        public string JobNo { get; set; }
        public string ProductCode { get; set; }
        public decimal QtyPerPkg { get; set; }
        /// <summary>
        /// InboundEntry: tdbcProductCode.Columns[2].Value
        /// DecimalNum must be filled in with the value from the selected PartMaster to perform validation 
        /// </summary>
        public decimal PartMasterDecimalNum { get; set; }
        public int PartMasterContainerFactor => int.Parse("1" + new string('0', (int)PartMasterDecimalNum));

        public int GetNoOfPackageAndLabel(decimal qty) => QtyPerPkg > 0 ? (int)(qty / QtyPerPkg) : 0;
    }
}
