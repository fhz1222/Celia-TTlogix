using Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.PetaPoco;
using PetaPoco;
using IMapper = AutoMapper.IMapper;

namespace Persistence.Repositories;

public class ILogIntegrationRepository : IILogIntegrationRepository
{
    private readonly Database dbContext;
    private readonly AppDbContext efContext;
    private readonly IMapper mapper;

    public ILogIntegrationRepository(IPPDbContextFactory factory, AppDbContext efContext, IMapper mapper)
    {
        dbContext = factory.GetInstance();
        this.efContext = efContext;
        this.mapper = mapper;
    }

    public string[] GetWarehouses()
    {
        return dbContext.Query<string>("SELECT WHSCode from dbi.iLogIntegrationWarehouse").ToArray();
    }

    public bool GetStatus()
    {
        return dbContext.SingleOrDefault<bool?>("SELECT TOP 1 IsEnabled from dbi.iLogIntegrationStatus") ?? false;
    }

    public void Disable()
    {
        efContext.Database.ExecuteSqlRaw("UPDATE dbi.iLogIntegrationStatus SET IsEnabled = 0");
    }

    public void Enable()
    {
        efContext.Database.ExecuteSqlRaw("UPDATE dbi.iLogIntegrationStatus SET IsEnabled = 1");
    }
}
