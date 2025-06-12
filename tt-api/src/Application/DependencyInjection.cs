using Application.Common.Models;
using Application.Interfaces.Services;
using Application.Interfaces.Utils;
using Application.Services;
using Application.Services.PalletPickerService;
using Application.UseCases.Common.Commands.AddCommand;
using Application.UseCases.Common.Commands.ToggleActiveCommand;
using Application.UseCases.Common.Commands.UpdateCommand;
using Application.UseCases.Common.Commands.UpsertCommand;
using Application.UseCases.Common.Queries.GetListQuery;
using Application.UseCases.CompanyProfiles;
using Application.UseCases.CompanyProfiles.Queries.GetCompanyProfiles;
using Application.UseCases.Customer;
using Application.UseCases.Registration;
using Application.UseCases.Registration.Commands.AddWarehouse;
using Application.UseCases.Registration.Commands.UpdateControlCode;
using Application.UseCases.Registration.Commands.UpdatePackageType;
using Application.UseCases.Registration.Commands.UpdateProductCode;
using Application.UseCases.Registration.Commands.UpdateUom;
using Application.UseCases.Registration.Commands.UpdateWarehouse;
using Application.UseCases.Registration.Queries.GetControlCodeList;
using Application.UseCases.Registration.Queries.GetPackageTypeList;
using Application.UseCases.Registration.Queries.GetProductCodeList;
using Application.UseCases.Registration.Queries.GetUomList;
using Application.UseCases.Registration.Queries.GetWarehouseList;
using Application.UseCases.Registration.Queries.GetAreaTypeList;
using Application.UseCases.StockTake;
using Application.UseCases.StockTake.Queries.GetStockTakeAnotherLocPid;
using Application.UseCases.StockTake.Queries.GetStockTakeInvalidPid;
using Application.UseCases.StockTake.Queries.GetStockTakeList;
using Application.UseCases.StockTake.Queries.GetStockTakeMissingPid;
using Application.UseCases.StockTake.Queries.GetStockTakeStandByLocations;
using Application.UseCases.StockTake.Queries.GetStockTakeUploadedList;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Application.UseCases.Registration.Commands.AddAreaType;
using Application.UseCases.Registration.Commands.UpdateAreaType;
using Application.UseCases.Registration.Commands.UpdateArea;
using Application.UseCases.Registration.Commands.AddArea;
using Application.UseCases.Registration.Queries.GetAreaList;
using Application.UseCases.Registration.Queries.GetLocationList;
using Application.UseCases.Registration.Commands.AddLocation;
using Application.UseCases.Registration.Queries.GetActiveAreasCombo;
using Application.UseCases.Registration.Queries.GetActiveWarehousesCombo;
using Application.UseCases.Registration.Queries.GetActiveAreaTypesCombo;
using Application.UseCases.Registration.Queries.GetAreaTypesCombo;
using Application.UseCases.Registration.Queries.GetLabelPrinterList;
using Application.UseCases.Registration.Commands.UpdateLabelPrinter;
using Application.UseCases.Registration.Commands.AddLabelPrinter;
using Application.UseCases.Registration.Queries.GetILogLocationCategoryCombo;
using Application.UseCases.Delivery.Queries.GetDeliveryCustomerClientList;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(Assembly.GetExecutingAssembly());

        services.AddTransientToggleActive<CompanyProfile>();
        services.AddTransientToggleActive<AddressBook>();
        services.AddTransientToggleActive<AddressContact>();

        services.AddTransientAddNew<AddWarehouseDto, Warehouse>();
        services.AddTransientAddNew<AddAreaTypeDto, AreaType>();
        services.AddTransientAddNew<AddAreaDto, Area>();
        services.AddTransientAddNew<AddLocationDto, LocationDto>();
        services.AddTransientAddNew<AddLabelPrinterDto, LabelPrinter>();

        services.AddTransientUpsert<UpsertCompanyProfileDto, CompanyProfile>();

        services.AddTransientUpdate<UpdateAddressBookDto, AddressBook>();
        services.AddTransientUpdate<UpdateAddressContactDto, AddressContact>();
        services.AddTransientUpdate<UpdatePackageTypeDto, PackageType>();
        services.AddTransientUpdate<UpdateProductCodeDto, ProductCode>();
        services.AddTransientUpdate<UpdateControlCodeDto, ControlCode>();
        services.AddTransientUpdate<UpdateUomDto, Uom>();
        services.AddTransientUpdate<UpdateWarehouseDto, Warehouse>();
        services.AddTransientUpdate<UpdateAreaTypeDto, AreaType>();
        services.AddTransientUpdate<UpdateAreaDto, Area>();
        services.AddTransientUpdate<UpdateLabelPrinterDto, LabelPrinter>();

        services.AddTransientGetList<GetCompanyProfilesDtoFilter, CompanyProfileDto>();
        services.AddTransientGetList<object, AddressBookDto>();
        services.AddTransientGetList<object, AddressContactDto>();
        services.AddTransientGetList<ControlCodeFilter, ControlCodeDto>();
        services.AddTransientGetList<ProductCodeFilter, ProductCodeDto>();
        services.AddTransientGetList<UomFilter, UomDto>();
        services.AddTransientGetList<GetStockTakeListDtoFilter, StockTakeDto>();
        services.AddTransientGetList<GetStockTakeUploadedListFilter, StockTakeItemDto>();
        services.AddTransientGetList<GetStockTakeInvalidPidFilter, StockTakeItemDto>();
        services.AddTransientGetList<GetStockTakeMissingPidFilter, StockTakeItemDto>();
        services.AddTransientGetList<GetStockTakeAnotherLocPidFilter, StockTakeItemDto>();
        services.AddTransientGetList<GetStockTakeStandByLocationsFilter, StockTakeLocationDto>();
        services.AddTransientGetList<GetPackageTypeListDtoFilter, PackageTypeDto>();
        services.AddTransientGetList<GetProductCodeListDtoFilter, ProductCodeListItemDto>();
        services.AddTransientGetList<GetControlCodeListDtoFilter, ControlCodeListItemDto>();
        services.AddTransientGetList<GetUomListDtoFilter, UomListItemDto>();
        services.AddTransientGetList<GetWarehouseListDtoFilter, WarehouseListItemDto>();
        services.AddTransientGetList<GetAreaTypeListDtoFilter, AreaTypeListItemDto>();
        services.AddTransientGetList<GetAreaListDtoFilter, AreaListItemDto>();
        services.AddTransientGetList<GetLocationListDtoFilter, LocationListItemDto>();
        services.AddTransientGetList<GetActiveAreasComboFilter, AreasComboDto>();
        services.AddTransientGetList<GetActiveAreaTypesComboFilter, AreaTypesComboDto>();
        services.AddTransientGetList<GetActiveWarehousesComboFilter, WarehouseComboDto>();
        services.AddTransientGetList<GetLabelPrinterListDtoFilter, LabelPrinterListItemDto>();
        services.AddTransientGetList<GetILogLocationCategoryComboFilter, ILogLocationCategoryComboDto>();
        services.AddTransientGetList<GetDeliveryCustomerClientListFilter, DeliveryCustomerClientDto>();

        services.AddTransientGet<Customer>();
        services.AddTransientGet<CustomerClient>();
        services.AddTransientGet<StockTake>();
        services.AddTransientGet<PackageType>();
        services.AddTransientGet<ProductCode>();
        services.AddTransientGet<ControlCode>();
        services.AddTransientGet<Uom>();
        services.AddTransientGet<Warehouse>();
        services.AddTransientGet<AreaType>();
        services.AddTransientGet<Area>();
        services.AddTransientGet<LocationDto>();
        services.AddTransientGet<LabelPrinter>();

        services.AddTransient<IJobNumberGenerator, JobNumberGenerator>();
        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IInventoryTransactionService, InventoryTransactionService>();
        services.AddTransient<IPIDGenerator, PIDNumberGenerator>();
        services.AddTransient<IPalletPicker, PalletPicker>();
        return services;
    }

    private static void AddTransientToggleActive<T>(this IServiceCollection services)
        where T : class
    {
        services.AddTransient<IRequestHandler<ToggleActiveCommand<T>, T>, ToggleActiveCommandHandler<T>>();
    }

    private static void AddTransientUpsert<DTO, T>(this IServiceCollection services)
        where DTO : class
        where T : class
    {
        services.AddTransient<IRequestHandler<UpsertCommand<DTO, T>, T>, UpsertCommandHandler<DTO, T>>();
    }

    private static void AddTransientUpdate<DTO, T>(this IServiceCollection services)
        where DTO : class
        where T : class
    {
        services.AddTransient<IRequestHandler<UpdateCommand<DTO, T>, T>, UpdateCommandHandler<DTO, T>>();
    }

    private static void AddTransientAddNew<DTO, T>(this IServiceCollection services)
        where DTO : class
        where T : class
    {
        services.AddTransient<IRequestHandler<AddCommand<DTO, T>, T>, AddCommandHandler<DTO, T>>();
    }

    private static void AddTransientGetList<FILTER, T>(this IServiceCollection services)
        where FILTER : class
        where T : class
    {
        services.AddTransient<IRequestHandler<GetListQuery<FILTER, T>, PaginatedList<T>>, GetListQueryHandler<FILTER, T>>();
    }

    private static void AddTransientGet<T>(this IServiceCollection services)
        where T : class
    {
        services.AddTransient<IRequestHandler<GetQuery<T>, T>, GetQueryHandler<T>>();
    }
}