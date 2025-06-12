using Application.Interfaces.Repositories;

namespace Application.Interfaces.Utils;

public interface IPIDGenerator
{
    string[] GetNewPIDs(IRepository repository, int noOfPids);
}
