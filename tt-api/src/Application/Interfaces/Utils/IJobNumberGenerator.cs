namespace Application.Interfaces.Utils;

public interface IJobNumberGenerator
{
    string GetJobNumber(IJobNumberSource jobNumberSource);
}
