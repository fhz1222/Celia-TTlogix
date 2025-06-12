namespace Domain.Enums;

public enum StockTransferReversalStatus
{
    New = 0,
    Processing = 1,
    Outstanding = 2,
    Completed = 8,
    Cancelled = 9,
    All = 10
}
