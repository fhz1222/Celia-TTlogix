using Application.Interfaces.Utils;

namespace Application.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}
