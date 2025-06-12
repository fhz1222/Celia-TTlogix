using Domain.Enums;

namespace Application.Interfaces.Utils;

public interface IJobNumberSource
{
    CodePrefix GetCodePrefix { get; }
    int GetLastJobNumber(string prefix);
}
