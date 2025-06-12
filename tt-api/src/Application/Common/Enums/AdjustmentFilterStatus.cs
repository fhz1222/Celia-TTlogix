namespace Application.Common.Enums;

public enum AdjustmentFilterStatus
{
    New = 0,
    Processing = 1,
    Completed = 2,
    Outstanding = 3, // virtual; reprents group of statuses: New and Processing
    Cancelled = 10,
    All = 11, // virtual 
}
