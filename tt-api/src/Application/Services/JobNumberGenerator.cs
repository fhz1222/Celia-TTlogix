using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;

namespace Application.Services;

public class JobNumberGenerator : IJobNumberGenerator
{
    private readonly IRepository repository;
    private readonly IDateTime dateTimeService;
    private const int JobNumberLength = 14;

    public JobNumberGenerator(IRepository repository, IDateTime dateTimeService)
    {
        this.repository = repository;
        this.dateTimeService = dateTimeService;
    }

    public string GetJobNumber(IJobNumberSource jobNumberSource)
    {
        var jobPrefix = repository.Utils.GetJobCode(jobNumberSource.GetCodePrefix);
        var currentDate = dateTimeService.Now;
        jobPrefix += currentDate.Year.ToString() + currentDate.Month.ToString().PadLeft(2, '0');
        var lastJobNumber = jobNumberSource.GetLastJobNumber(jobPrefix);
        return jobPrefix + (lastJobNumber + 1).ToString().PadLeft(JobNumberLength - jobPrefix.Length, '0');
    }
}
