namespace Application.Common.Enums;

public enum OutboundStatus
{
    NewJob = 0,
    PartialDownload = 1,
    Downloaded = 2,
    PartialPicked = 3,
    Picked = 4,
    Packed = 5,
    OutStanding = 6,
    InTransit = 7,
    Completed = 8,
    Cancelled = 9,
    Discrepancy = 10,
    All = 11,
    // TESAG wh transfer status
    TruckDeparture = 12,
    DisrepancyStorage = 23,
}
