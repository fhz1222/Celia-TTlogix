namespace Application.Common.Enums;

public enum DecantFilterStatus
{
    New = 0,
    Processing = 1,
    Outstanding = 7, // virtual; reprents group of statuses: New and Processing
    Completed = 8,
    Cancelled = 9,
    All = 10, // virtual 
}
