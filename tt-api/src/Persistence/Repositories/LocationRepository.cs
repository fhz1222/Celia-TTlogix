using Application.Extensions;
using Application.Interfaces.Repositories;
using Application.UseCases.ILogLocationIntegration;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.Repositories;

public class LocationRepository : ILocationRepository
{
    private class ILogLocationCategory
    {
        public static readonly string InboundLocation = "Inbound";

        public static readonly string ILogStorageLocation = "iLogStorage";

        public static readonly string Outbound = "Outbound";

        public static readonly string ILogTransfer = "iLogTransfer";

        public static readonly string OtherTTLogixManaged = "OtherTTLogixManaged";
    }

    private readonly AppDbContext context;

    public LocationRepository(AppDbContext context)
    {
        this.context = context;
    }

    public string GetILogLocationCategoryName(string locationCode, string whsCode)
    {
        var loc = context.TtLocations.AsNoTracking().First(l => l.Code == locationCode && l.Whscode == whsCode);
        var name = context.ILogLocationCategories.AsNoTracking().First(c => c.Id == loc.ILogLocationCategoryId).Name;
        return name;
    }

    public bool IsILogInboundLocation(string locationCode, string whsCode)
    {
        var loc = context.TtLocations.AsNoTracking().First(l => l.Code == locationCode && l.Whscode == whsCode);
        var catId = context.ILogLocationCategories.AsNoTracking().First(c => c.Name == ILogLocationCategory.InboundLocation).Id;
        return loc.ILogLocationCategoryId == catId;
    }

    public bool IsILogStorageLocation(string locationCode, string whsCode)
    {
        var loc = context.TtLocations.AsNoTracking().First(l => l.Code == locationCode && l.Whscode == whsCode);
        var catId = context.ILogLocationCategories.AsNoTracking().First(c => c.Name == ILogLocationCategory.ILogStorageLocation).Id;
        return loc.ILogLocationCategoryId == catId;
    }

    public bool IsLocationOfCategory(string locationCode, string whsCode, string categoryName)
    {
        var loc = context.TtLocations.AsNoTracking().First(l => l.Code == locationCode && l.Whscode == whsCode);
        var cat = context.ILogLocationCategories.AsNoTracking().First(c => c.Id == loc.ILogLocationCategoryId).Name;
        return categoryName == cat;
    }

    public int GetILogStorageLocationCategoryId()
    {
        return context.ILogLocationCategories.AsNoTracking().First(c => c.Name == ILogLocationCategory.ILogStorageLocation).Id;
    }

    public int GetILogInboundLocationCategoryId()
    {
        return context.ILogLocationCategories.AsNoTracking().First(c => c.Name == ILogLocationCategory.InboundLocation).Id;
    }

    private int GetOutboundLocationCategoryId()
    {
        return context.ILogLocationCategories.AsNoTracking().First(c => c.Name == ILogLocationCategory.Outbound).Id;
    }

    private int GetILogTransferLocationCategoryId()
    {
        return context.ILogLocationCategories.AsNoTracking().First(c => c.Name == ILogLocationCategory.ILogTransfer).Id;
    }

    private int GetILogOtherTTLogixManagedLocationCategoryId()
    {
        return context.ILogLocationCategories.AsNoTracking().First(c => c.Name == ILogLocationCategory.OtherTTLogixManaged).Id;
    }

    public Location? GetLocation(string locationCode, string whsCode)
    {
        var loc = context.TtLocations.AsNoTracking().FirstOrDefault(l => l.Code == locationCode && l.Whscode == whsCode);
        if (loc == null) return null;
        return new Location
        {
            Code = loc.Code,
            Name = loc.Name,
            WarehouseCode = loc.Whscode,
            Type = LocationType.From(loc.Type),
            IsActive = loc.Status == 1
        };
    }

    public void ResetLocationCategories()
    {
        context.Database.ExecuteSqlRaw("UPDATE TT_Location SET ILogLocationCategoryId = 0");
    }

    public void RestoreILogSystemLocations(string whsCode)
    {
        var transferCategoryId = GetILogTransferLocationCategoryId();
        var transferLocation = context.TtLocations.FirstOrDefault(t => t.Whscode == whsCode && t.Code == ILogLocationCategory.ILogTransfer);
        if (transferLocation is { })
        {
            transferLocation.ILogLocationCategoryId = transferCategoryId;
        }
        else
        {
            context.Add(new TtLocation()
            {
                Code = ILogLocationCategory.ILogTransfer,
                Whscode = whsCode,
                AreaCode = "Line",
                Name = ILogLocationCategory.ILogTransfer,
                M3 = 1,
                CreatedBy = "admin",
                ILogLocationCategoryId = transferCategoryId
            });
        }

        var outboundCategoryId = GetOutboundLocationCategoryId();
        var outboundLocation = context.TtLocations.FirstOrDefault(t => t.Whscode == whsCode && t.Code == ILogLocationCategory.Outbound);
        if (outboundLocation is { })
        {
            outboundLocation.ILogLocationCategoryId = outboundCategoryId;
        }
        else
        {
            context.Add(new TtLocation()
            {
                Code = ILogLocationCategory.Outbound,
                Whscode = whsCode,
                AreaCode = "Line",
                Name = ILogLocationCategory.Outbound,
                M3 = 1,
                CreatedBy = "admin",
                ILogLocationCategoryId = outboundCategoryId
            });
        }
    }

    public IEnumerable<ILogIntegrationLocationDto> GetLocationsForWHS(string[] WHSCodes)
    {
        var locations = context.TtLocations.AsNoTracking()
            .Where(x => WHSCodes.Contains(x.Whscode))
            .Select(x => new ILogIntegrationLocationDto()
            {
                Code = x.Code,
                AreaCode = x.AreaCode,
                Whscode = x.Whscode,
                ILogLocationCategoryId = x.ILogLocationCategoryId,
                IsActive = x.Status == (byte)LocationStatus.Active
            })
            .ToList();
        return locations;
    }

    public void AddLocations(IEnumerable<ILogIntegrationLocationDto> items)
    {
        var locations = items
            .Select(x => new TtLocation()
            {
                Code = x.Code,
                Whscode = x.Whscode,
                AreaCode = x.AreaCode,
                Name = x.Code,
                M3 = 1,
                CreatedBy = "admin",
                ILogLocationCategoryId = x.ILogLocationCategoryId
            });
        context.TtLocations.AddRange(locations);
    }

    public void ActivateLocations(IEnumerable<ILogIntegrationLocationIdDto> items)
    {
        var keys = items.Select(i => new { i.Code, i.Whscode }).ToList();
        var whs = items.Select(i => i.Whscode).Distinct().ToList();

        var locations = context.TtLocations
            .Where(l => whs.Contains(l.Whscode))
            .AsEnumerable()
            .Where(l => keys.Contains(new { l.Code, l.Whscode }))
            .ToList();

        foreach(var location in locations)
        {
            location.Status = (byte)LocationStatus.Active;
        }
    }

    public void DeactivateLocations(IEnumerable<ILogIntegrationLocationIdDto> items)
    {
        var keys = items.Select(i => new { i.Code, i.Whscode }).ToList();
        var whs = items.Select(i => i.Whscode).Distinct().ToList();

        var locations = context.TtLocations
            .Where(l => whs.Contains(l.Whscode))
            .AsEnumerable()
            .Where(l => keys.Contains(new { l.Code, l.Whscode }))
            .ToList();

        var otherTTLogixManagedId = GetILogOtherTTLogixManagedLocationCategoryId();
        foreach(var location in locations)
        {
            location.Status = (byte)LocationStatus.Inactive;
            location.ILogLocationCategoryId = otherTTLogixManagedId;
        }
    }

    public void UpdateLocations(IEnumerable<ILogIntegrationLocationDto> items)
    {
        var whs = items.Select(i => i.Whscode).Distinct().ToList();

        var locations = context.TtLocations
            .Where(l => whs.Contains(l.Whscode))
            .AsEnumerable()
            .Join(items, l => new { l.Code, l.Whscode }, i => new { i.Code, i.Whscode }, (l, i) => new { l, i })
            .ToList();

        foreach(var data in locations)
        {
            data.l.AreaCode = data.i.AreaCode;
            data.l.ILogLocationCategoryId = data.i.ILogLocationCategoryId;
            data.l.Status = (byte)LocationStatus.Active;
        }
    }
}
