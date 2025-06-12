using Application.Common.Models;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Entities;
using Persistence.FilterHandlers;
using Persistence.FilterHandlers.Area;
using Persistence.FilterHandlers.Delivery;
using Persistence.FilterHandlers.StockTakeTabs;
using EntityType = Application.UseCases.Common.EntityType;

namespace Persistence.Repositories;

public class MetadataRepository : IMetadataRepository
{
    private readonly AppDbContext context;
    private readonly IMapper mapper;
    private readonly IServiceProvider provider;

    private static Type ParseType(EntityType entityType) => entityType switch
    {
        EntityType.CompanyProfile => typeof(TtCompanyProfile),
        EntityType.AddressBook => typeof(TtAddressBook),
        EntityType.AddressContact => typeof(TtAddressContact),
        EntityType.ControlCode => typeof(TtControlCode),
        EntityType.ProductCode => typeof(TtProductCode),
        EntityType.Customer => typeof(TtCustomer),
        EntityType.Uom => typeof(TtUOM),
        EntityType.CustomerClient => typeof(TtCustomerClient),
        EntityType.StockTake => typeof(TtStockTakeByLoc),
        EntityType.StockTakeItem => typeof(StockTakeItemWithStorageInfo),
        EntityType.Location => typeof(TtLocation),
        EntityType.StockTakeRelocationLog => typeof(TtStockTakeRelocationLog),
        EntityType.PackageType => typeof(TtPackageType),
        EntityType.Warehouse => typeof(TtWarehouse),
        EntityType.AreaType => typeof(TtAreaType),
        EntityType.Area => typeof(TtArea),
        EntityType.AreaItem => typeof(AreaWithTypeName),
        EntityType.LabelPrinter => typeof(TtLabelPrinter),
        EntityType.ILogLocationCategory => typeof(ILogLocationCategory),
        EntityType.DeliveryCustomerClient => typeof(DeliveryCustomerClient),
        _ => throw new NotImplementedException()
    };

#pragma warning disable IDE0051 // Remove unused private members
    private PaginatedList<T> GetPaginatedListInternal<F, T, DBT>(F? filter, OrderBy? sorting, PaginationQuery? pagination)
#pragma warning restore IDE0051 // Remove unused private members
    {
        var filterHandler = provider.GetService<IFilter<F, DBT>>()
            ?? throw new ApplicationError("Cannot find filter handler.");

        var query = filterHandler.GetFilteredTable(context, filter);

        var sorted = query.ApplyOrderBy(sorting);

        var items = sorted
            .Paginate(pagination, out var count)
            .ProjectTo<T>(mapper.ConfigurationProvider)
            .ToList();

        return new PaginatedList<T>(items, count, pagination?.PageNumber ?? 1, pagination?.ItemsPerPage ?? count);
    }

    public MetadataRepository(AppDbContext context, IMapper mapper, IServiceProvider provider)
    {
        this.context = context;
        this.mapper = mapper;
        this.provider = provider;
    }

    public T? Get<T>(EntityType entityType, params string[] key)
    {
        var dbType = ParseType(entityType);
        var obj = context.Find(dbType, key);
        return mapper.Map<T>(obj);
    }

    public void AddNew<T>(EntityType entityType, T obj) where T : class
    {
        var dbType = ParseType(entityType);
        var dbObj = mapper.Map(obj, obj.GetType(), dbType);
        context.Add(dbObj);
    }

    public void Update<T>(EntityType entityType, T updated, params string[] key)
    {
        var dbType = ParseType(entityType);
        var existing = context.Find(dbType, key)
            ?? throw new ApplicationError($"Cannot find {entityType} for {string.Join(' ', key)}");

        mapper.Map(updated, existing);
    }

    public void AddNewWithMetadata<T>(EntityType entityType, T obj, Metadata metadata) where T : class
    {
        var dbType = ParseType(entityType);
        var dbObj = mapper.Map(metadata, metadata.GetType(), dbType);
        mapper.Map(obj, dbObj);
        context.Add(dbObj);
    }

    public void UpdateWithMetadata<T>(EntityType entityType, T updated, Metadata metadata, string[] key) where T : class
    {
        var dbType = ParseType(entityType);
        var existing = context.Find(dbType, key)
            ?? throw new EntityDoesNotExistException();

        mapper.Map(metadata, existing);
        mapper.Map(updated, existing);
    }

    public void Remove(EntityType entityType, string[] key)
    {
        var dbType = ParseType(entityType);
        var existing = context.Find(dbType, key)
            ?? throw new EntityDoesNotExistException();
        context.Remove(existing);
    }

    public PaginatedList<T> GetPaginatedList<F, T>(EntityType entityType, F? filter, OrderBy? sorting, PaginationQuery? pagination)
    {
        var dbType = ParseType(entityType);
        var retVal = (PaginatedList<T>)(typeof(MetadataRepository)
            .GetMethod("GetPaginatedListInternal", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            ?.MakeGenericMethod(typeof(F), typeof(T), dbType)
            ?.Invoke(this, new object?[] { filter, sorting, pagination })
            ?? throw new ApplicationError("Error while trying to call MetadataRepository.GetPaginatedList"));
        return retVal;
    }
}
