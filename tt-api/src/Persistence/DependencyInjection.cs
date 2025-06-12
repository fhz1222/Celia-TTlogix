using Application.Interfaces.Repositories;
using Application.UseCases.CompanyProfiles.Queries.GetCompanyProfiles;
using Application.UseCases.Customer;
using Application.UseCases.Registration.Queries.GetControlCodeList;
using Application.UseCases.Registration.Queries.GetPackageTypeList;
using Application.UseCases.Registration.Queries.GetProductCodeList;
using Application.UseCases.Registration.Queries.GetUomList;
using Application.UseCases.Registration.Queries.GetWarehouseList;
using Application.UseCases.Registration.Queries.GetAreaTypeList;
using Application.UseCases.StockTake.Queries.GetStockTakeAnotherLocPid;
using Application.UseCases.StockTake.Queries.GetStockTakeInvalidPid;
using Application.UseCases.StockTake.Queries.GetStockTakeList;
using Application.UseCases.StockTake.Queries.GetStockTakeMissingPid;
using Application.UseCases.StockTake.Queries.GetStockTakeStandByLocations;
using Application.UseCases.StockTake.Queries.GetStockTakeUploadedList;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Entities;
using Persistence.FilterHandlers;
using Persistence.FilterHandlers.StockTakeTabs;
using Persistence.PetaPoco;
using Persistence.Repositories;
using System.Reflection;
using Persistence.FilterHandlers.Area;
using Application.UseCases.Registration.Queries.GetAreaList;
using Persistence.FilterHandlers.Location;
using Application.UseCases.Registration.Queries.GetLocationList;
using Application.UseCases.Registration.Queries.GetActiveAreasCombo;
using Application.UseCases.Registration.Queries.GetActiveWarehousesCombo;
using Application.UseCases.Registration.Queries.GetAreaTypesCombo;
using Application.UseCases.Registration.Queries.GetActiveAreaTypesCombo;
using Application.UseCases.Registration.Queries.GetLabelPrinterList;
using Application.UseCases.Registration.Queries.GetILogLocationCategoryCombo;
using Application.UseCases.Delivery.Queries.GetDeliveryCustomerClientList;
using Persistence.FilterHandlers.Delivery;

namespace Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("Database"), o => o.CommandTimeout(60));
        });

        services.AddScoped<IPPDbContextFactory, PetaPocoDbContextFactory>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IStorageDetailRepository, StorageDetailRepository>();
        services.AddScoped<IAdjustmentRepository, AdjustmentRepository>();
        services.AddScoped<IAdjustmentItemRepository, AdjustmentItemRepository>();
        services.AddScoped<IUtilsRepository, UtilsRepository>();
        services.AddScoped<IQuarantineRepository, QuarantineRepository>();
        services.AddScoped<IRelocationRepository, RelocationRepository>();
        services.AddScoped<IDecantRepository, DecantRepository>();
        services.AddScoped<IRepository, Repository>();
        services.AddScoped<ILabelRepository, LabelsRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IPalletTransferRequestsRepository, PalletTransferRequestsRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IPickingRepository, PickingRepository>();
        services.AddScoped<IILogIntegrationRepository, ILogIntegrationRepository>();
        services.AddScoped<IStockTransferRepository, StockTransferRepository>();
        services.AddScoped<IILogBoxRepository, ILogBoxRepository>();
        services.AddScoped<IILogPickingRequestRepository, ILogPickingRequestRepository>();
        services.AddScoped<IOutboundRepository, OutboundRepository>();
        services.AddScoped<IInvoiceRequestRepository, InvoiceRequestRepository>();
        services.AddScoped<IInboundReversalRepository, InboundReversalRepository>();
        services.AddScoped<IBillingLogRepository, BillingLogRepository>();
        services.AddScoped<IStockTransferReversalRepository, StockTransferReversalRepository>();
        services.AddScoped<IInboundRepository, InboundRepository>();
        services.AddScoped<ICompanyProfileRepository, CompanyProfileRepository>();

        services.AddTransient<IFilter<GetCompanyProfilesDtoFilter, TtCompanyProfile>, GetCompanyProfilesDtoFilterHandler>();
        services.AddTransient<IFilter<object, TtAddressBook>, EmptyAddressBookFilterHandler>();
        services.AddTransient<IFilter<object, TtAddressContact>, EmptyAddressContactFilterHandler>();
        services.AddTransient<IFilter<ControlCodeFilter, TtControlCode>, ControlCodeFilterHandler>();
        services.AddTransient<IFilter<ProductCodeFilter, TtProductCode>, ProductCodeFilterHandler>();
        services.AddTransient<IFilter<UomFilter, TtUOM>, UomFilterHandler>();
        services.AddTransient<IFilter<GetStockTakeListDtoFilter, TtStockTakeByLoc>, GetStockTakeListDtoFilterHandler>();
        services.AddTransient<IFilter<GetStockTakeInvalidPidFilter, StockTakeItemWithStorageInfo>, GetStockTakeInvalidPidFilterHandler>();
        services.AddTransient<IFilter<GetStockTakeMissingPidFilter, StockTakeItemWithStorageInfo>, GetStockTakeMissingPidFilterHandler>();
        services.AddTransient<IFilter<GetStockTakeAnotherLocPidFilter, StockTakeItemWithStorageInfo>, GetStockTakeAnotherLocPidFilterHandler>();
        services.AddTransient<IFilter<GetStockTakeUploadedListFilter, StockTakeItemWithStorageInfo>, GetStockTakeUploadedListFilterHandler>();
        services.AddTransient<IFilter<GetStockTakeStandByLocationsFilter, TtLocation>, GetStockTakeStandByLocationsFilterHandler>();
        services.AddTransient<IFilter<GetPackageTypeListDtoFilter, TtPackageType>, GetPackageTypeListDtoFilterHandler>();
        services.AddTransient<IFilter<GetProductCodeListDtoFilter, TtProductCode>, GetProductCodeListDtoFilterHandler>();
        services.AddTransient<IFilter<GetControlCodeListDtoFilter, TtControlCode>, GetControlCodeListDtoFilterHandler>();
        services.AddTransient<IFilter<GetUomListDtoFilter, TtUOM>, GetUomListDtoFilterHandler>();
        services.AddTransient<IFilter<GetWarehouseListDtoFilter, TtWarehouse>, GetWarehouseListDtoFilterHandler>();
        services.AddTransient<IFilter<GetAreaTypeListDtoFilter, TtAreaType>, GetAreaTypeListDtoFilterHandler>();
        services.AddTransient<IFilter<GetAreaListDtoFilter, AreaWithTypeName>, GetAreaListDtoFilterHandler>();
        services.AddTransient<IFilter<GetLocationListDtoFilter, TtLocation>, GetLocationListDtoFilterHandler>();
        services.AddTransient<IFilter<GetActiveAreasComboFilter, TtArea>, GetActiveAreasComboFilterHandler>();
        services.AddTransient<IFilter<GetActiveAreaTypesComboFilter, TtAreaType>, GetActiveAreaTypesComboFilterHandler>();
        services.AddTransient<IFilter<GetActiveWarehousesComboFilter, TtWarehouse>, GetActiveWarehousesComboFilterHandler>();
        services.AddTransient<IFilter<GetLabelPrinterListDtoFilter, TtLabelPrinter>, GetLabelPrinterListDtoFilterHandler>();
        services.AddTransient<IFilter<GetILogLocationCategoryComboFilter, ILogLocationCategory>, GetILogLocationCategoryComboFilterHandler>();
        services.AddTransient<IFilter<GetDeliveryCustomerClientListFilter, DeliveryCustomerClient>, GetDeliveryCustomerClientListFilterHandler>();

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}