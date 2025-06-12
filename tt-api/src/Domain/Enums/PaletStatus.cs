namespace Domain.Enums;

//TODO typo
public enum PaletStatus
{
    Incoming = 0,
    Putaway = 1,
    Allocated = 2,
    Picked = 3,
    Packed = 4,
    InTransit = 5,
    Dispatched = 6,
    Splitted = 7,
    Quarantine = 9,

    //Temporary status before Allocated
    Allocating = 10,
    Kitting = 11,
    Splitting = 12,

    //Status 21 - 30 is reserved for item locked by warehouse operation
    Transferring = 21,
    Decant = 22,
    Discrepancy = 23,

    DiscrepancyFixed = 96,
    Reversal = 97,
    ZeroOut = 98,
    Cancelled = 99
}