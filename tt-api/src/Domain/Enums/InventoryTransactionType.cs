namespace Domain.Enums;

public enum InventoryTransactionType
{
    Inbound = 0,
    Outbound = 1,
    PositiveAdjustment = 2,
    NegativeAdjustment = 3,
    PurchaseOfAgedStock = 4,
    SalesOfAgedStock = 5,
    ReversalOfReturn = 6,
    ReversalOfInbound = 7,
    ReversalOfAdjustment = 8,
    ReversalOfSaleOfAgeStock = 9,
    ReversalOfPurchaseOfAgeStock = 10,
    ExternalSystemInbound = 11,
    ExternalSystemOutbound = 12,
    //ExternalSystemCallOff means outbound completed in VMI hub but stock get from ELX return area.
    ExternalSystemCallOff = 13,
    WHSTransferIn = 14,
    WHSTransferOut = 15,
}
