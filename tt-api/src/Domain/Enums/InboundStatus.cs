using System.ComponentModel;

namespace Domain.Enums;

public enum InboundStatus
{
    [Description("New Job")]
    NewJob = 0,
    [Description("Partial Download")]
    PartialDownload = 1,
    [Description("Downloaded")]
    Downloaded = 2,
    [Description("Partial Putaway")]
    PartialPutaway = 3,
    [Description("Outstanding")]
    Outstanding = 4,
    [Description("Completed")]
    Completed = 8,
    [Description("Cancelled")]
    Cancelled = 9,
    [Description("All")]
    All = 10
}