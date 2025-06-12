namespace Domain.Enums;

public enum StockTransferStatus
{
    New = 0,
    Processing = 1,
    Outstanding = 7,
    Completed = 8,
    Cancelled = 10,
    All = 11
}
