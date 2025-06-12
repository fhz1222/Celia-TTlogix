namespace TT.Core.QueryResults
{
    public class StockTransferTotalValueQueryResult
    {
        public string Currency { get; set; }
        public decimal OutboundTotalValue { get; set; }
        public bool MixedCurrency { get; set; }
    }
}