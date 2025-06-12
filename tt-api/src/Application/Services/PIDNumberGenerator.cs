using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;

namespace Application.Services;

public class PIDNumberGenerator : IPIDGenerator
{
    private readonly IDateTime dateTimeService;
    private readonly string OwnerCode;
    private const int PID_LENGHT = 17;
    private string? ownerPrefix;

    public PIDNumberGenerator(IDateTime dateTimeService, IAppSettings appSettings)
    {
        this.dateTimeService = dateTimeService;
        OwnerCode = appSettings.OwnerCode;
    }

    public string[] GetNewPIDs(IRepository repository, int noOfPids)
    {
        if (ownerPrefix == null)
        {
            SetOwnerPrefix(repository);
        }

        var currentDate = dateTimeService.Now;

        var prefix = ownerPrefix + currentDate.Year.ToString() + currentDate.Month.ToString().PadLeft(2, '0');
        var lastPID = repository.StorageDetails.GetLastPIDNumber(prefix);
        lastPID = lastPID == null ? "0" : lastPID.Replace(prefix, string.Empty);
        var uintNumber = checked(Convert.ToUInt32(lastPID, 16));
        var newPids = new string[noOfPids];
        for (int i = 0; i < noOfPids; i++)
        {
            uintNumber++;
            var newPid = prefix + String.Format("{0:x2}", uintNumber).ToString().PadLeft(PID_LENGHT - prefix.Length, '0').ToUpper();
            repository.StorageDetails.AddNewPIDCode(newPid, currentDate);
            newPids[i] = newPid;
        }
        return newPids;
    }

    private void SetOwnerPrefix(IRepository repository)
    {
        var owner = repository.Utils.GetOwner(OwnerCode);
        if (owner == null)
        {
            throw new UnknownOwnerCodeException();
        }

        ownerPrefix = owner.Site;
    }
}
