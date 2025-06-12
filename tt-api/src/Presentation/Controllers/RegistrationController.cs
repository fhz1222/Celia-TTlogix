using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using Presentation.Common;
using Presentation.Configuration;
using Application.UseCases.Common;
using Application.UseCases.Common.Queries.GetListQuery;
using Application.Common.Models;
using Presentation.Utilities;
using Application.UseCases.Registration;
using Application.UseCases.Registration.Queries.GetPackageTypeList;
using Application.UseCases.Registration.Commands.UpdatePackageType;
using Application.UseCases.Common.Commands.UpdateCommand;
using Application.UseCases.Registration.Commands.AddPackageType;
using Application.UseCases.Registration.Commands.AddProductCode;
using Application.UseCases.Registration.Commands.UpdateProductCode;
using Application.UseCases.Registration.Queries.GetProductCodeList;
using Application.UseCases.Registration.Commands.UpdateControlCode;
using Application.UseCases.Registration.Commands.AddControlCode;
using Application.UseCases.Registration.Queries.GetControlCodeList;
using Application.UseCases.Registration.Commands.UpdateUom;
using Application.UseCases.Registration.Commands.AddUom;
using Application.UseCases.Registration.Queries.GetUomList;
using Application.UseCases.Registration.Commands.UpdateWarehouse;
using Application.UseCases.Registration.Commands.AddWarehouse;
using Application.UseCases.Registration.Queries.GetWarehouseList;
using Application.UseCases.Registration.Commands.UpdateAreaType;
using Application.UseCases.Registration.Commands.AddAreaType;
using Application.UseCases.Registration.Queries.GetAreaTypeList;
using Application.UseCases.Registration.Commands.UpdateArea;
using Application.UseCases.Registration.Commands.AddArea;
using Application.UseCases.Registration.Queries.GetAreaList;
using Application.UseCases.Registration.Commands.UpdateLocation;
using Application.UseCases.Registration.Commands.AddLocation;
using Application.UseCases.Registration.Queries.GetLocationList;
using Application.UseCases.Common.Commands.AddCommand;
using Application.UseCases.Customer;
using Domain.ValueObjects;
using Application.UseCases.Registration.Queries.GetActiveAreasCombo;
using Application.UseCases.Registration.Queries.GetAreaTypesCombo;
using Application.UseCases.Registration.Queries.GetActiveWarehousesCombo;
using Application.UseCases.Registration.Queries.GetActiveAreaTypesCombo;
using Application.UseCases.Registration.Queries.GetLabelPrinterList;
using Application.UseCases.Registration.Commands.AddLabelPrinter;
using Application.UseCases.Registration.Commands.UpdateLabelPrinter;
using Application.UseCases.Registration.Queries.GetILogLocationCategoryCombo;
using Application.UseCases.Registration.Commands.ToggleActiveLocation;
using Application.UseCases.StockTransferReversals;
using Application.UseCases.Registration.Commands.DeleteLabelPrinter;

namespace Presentation.Controllers;
/// <summary>
/// Registration Controller provides method to get registration data
/// </summary>
public partial class RegistrationController : ApiControllerBase
{
    private readonly IMapper mapper;
    private readonly IFeatureManager manager;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="manager"></param>
    public RegistrationController(IMapper mapper, IFeatureManager manager)
    {
        this.mapper = mapper;
        this.manager = manager;
    }

    [HttpGet("isActive")]
    public async Task<bool> IsActive()
        => await manager.IsEnabledAsync(FeatureFlags.Registration);

    /// <summary>
    /// Gets active warehouses for combo list
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpGet("getActiveWarehouses")]
    public async Task<IEnumerable<WarehouseComboDto>> GetActiveWarehouses()
    {
        var result = await Mediator.Send(new GetListQuery<GetActiveWarehousesComboFilter, WarehouseComboDto>()
        {
            Filter = new GetActiveWarehousesComboFilter { Status = Status.Active },
            EntityType = EntityType.Warehouse,
            Sorting = null,
            Pagination = null,
        });
        return result.Items;
    }

    /// <summary>
    /// Gets active Areas for combo list
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpGet("getActiveAreas")]
    public async Task<IEnumerable<AreasComboDto>> GetActiveAreas(string warehouseCode)
    {
        var result = await Mediator.Send(new GetListQuery<GetActiveAreasComboFilter, AreasComboDto>()
        {
            Filter = new GetActiveAreasComboFilter { WhsCode = warehouseCode, Status = Status.Active },
            EntityType = EntityType.Area,
            Sorting = null,
            Pagination = null,
        });
        return result.Items;
    }

    /// <summary>
    /// Gets active AreaTypes for combo list
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpGet("getActiveAreaTypes")]
    public async Task<IEnumerable<AreaTypesComboDto>> GetActiveAreaTypes()
    {
        var result = await Mediator.Send(new GetListQuery<GetActiveAreaTypesComboFilter, AreaTypesComboDto>()
        {
            Filter = new GetActiveAreaTypesComboFilter { Status = Status.Active },
            EntityType = EntityType.AreaType,
            Sorting = null,
            Pagination = null,
        });
        return result.Items;
    }

    /// <summary>
    /// Gets ilog location category for combo list
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpGet("getILogLocationCategories")]
    public async Task<IEnumerable<ILogLocationCategoryComboDto>> GetILogLocationCategories()
    {
        var result = await Mediator.Send(new GetListQuery<GetILogLocationCategoryComboFilter, ILogLocationCategoryComboDto>()
        {
            Filter = new GetILogLocationCategoryComboFilter { },
            EntityType = EntityType.ILogLocationCategory,
            Sorting = null,
            Pagination = null,
        });
        return result.Items;
    }

    #region PackageType

    /// <summary>
    /// Get package type list
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpGet("getPackageTypeList")]
    public async Task<IEnumerable<PackageTypeDto>> GetPackageTypeList([FromQuery] GetRegistrationListParameters parameters)
    {
        var gridFilter = mapper.Map<GetPackageTypeListDtoFilter>(parameters);
        var result = await Mediator.Send(new GetListQuery<GetPackageTypeListDtoFilter, PackageTypeDto>()
        {
            Filter = gridFilter,
            EntityType = EntityType.PackageType,
            Sorting = parameters.Sorting,
            Pagination = null,
        });
        return result.Items;
    }

    /// <summary>
    /// Get package type
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpGet("getPackageType")]
    public async Task<PackageType> GetPackageType(string code)
    {
        var result = await Mediator.Send(new GetQuery<PackageType>()
        {
            Key = new string[] { code },
            EntityType = EntityType.PackageType,
        });
        return result;
    }

    /// <summary>
    /// Add package type
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpPost("addPackageType")]
    public async Task<PackageType> AddPackageType([FromBody] AddPackageTypeDto dto, string userCode)
    {
        var result = await Mediator.Send(new AddPackageTypeCommand()
        {
            Dto = dto,
            UserCode = userCode,
        });
        return result;
    }

    /// <summary>
    /// Update package type
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpPost("updatePackageType")]
    public async Task<PackageType> UpdatePackageType([FromBody] UpdatePackageTypeDto dto, string userCode)
    {
        var result = await Mediator.Send(new UpdateCommand<UpdatePackageTypeDto, PackageType>()
        {
            Key = new string[] { dto.Code },
            Updated = dto,
            EntityType = EntityType.PackageType,
            UserCode = userCode,
        });
        return result;
    }

    #endregion

    #region ProductCode

    /// <summary>
    /// Get ProductCode list
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpGet("getProductCodeList")]
    public async Task<IEnumerable<ProductCodeListItemDto>> GetProductCodeList([FromQuery] GetRegistrationListParameters parameters)
    {
        var gridFilter = mapper.Map<GetProductCodeListDtoFilter>(parameters);
        var result = await Mediator.Send(new GetListQuery<GetProductCodeListDtoFilter, ProductCodeListItemDto>()
        {
            Filter = gridFilter,
            EntityType = EntityType.ProductCode,
            Sorting = parameters.Sorting,
            Pagination = null,
        });
        return result.Items;
    }

    /// <summary>
    /// Get ProductCode
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpGet("getProductCode")]
    public async Task<ProductCode> GetProductCode(string code)
    {
        var result = await Mediator.Send(new GetQuery<ProductCode>()
        {
            Key = new string[] { code },
            EntityType = EntityType.ProductCode,
        });
        return result;
    }

    /// <summary>
    /// Add ProductCode
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpPost("addProductCode")]
    public async Task<ProductCode> AddProductCode([FromBody] AddProductCodeDto dto, string userCode)
    {
        var result = await Mediator.Send(new AddProductCodeCommand()
        {
            Dto = dto,
            UserCode = userCode,
        });
        return result;
    }

    /// <summary>
    /// Update ProductCode
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpPost("updateProductCode")]
    public async Task<ProductCode> UpdateProductCode([FromBody] UpdateProductCodeDto dto, string userCode)
    {
        var result = await Mediator.Send(new UpdateCommand<UpdateProductCodeDto, ProductCode>()
        {
            Key = new string[] { dto.Code },
            Updated = dto,
            EntityType = EntityType.ProductCode,
            UserCode = userCode,
        });
        return result;
    }

    #endregion

    #region ControlCode

    /// <summary>
    /// Get ControlCode list
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpGet("getControlCodeList")]
    public async Task<IEnumerable<ControlCodeListItemDto>> GetControlCodeList([FromQuery] GetRegistrationListParameters parameters)
    {
        var gridFilter = mapper.Map<GetControlCodeListDtoFilter>(parameters);
        var result = await Mediator.Send(new GetListQuery<GetControlCodeListDtoFilter, ControlCodeListItemDto>()
        {
            Filter = gridFilter,
            EntityType = EntityType.ControlCode,
            Sorting = parameters.Sorting,
            Pagination = null,
        });
        return result.Items;
    }

    /// <summary>
    /// Get ControlCode
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpGet("getControlCode")]
    public async Task<ControlCode> GetControlCode(string code)
    {
        var result = await Mediator.Send(new GetQuery<ControlCode>()
        {
            Key = new string[] { code },
            EntityType = EntityType.ControlCode,
        });
        return result;
    }

    /// <summary>
    /// Add ControlCode
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpPost("addControlCode")]
    public async Task<ControlCode> AddControlCode([FromBody] AddControlCodeDto dto, string userCode)
    {
        var result = await Mediator.Send(new AddControlCodeCommand()
        {
            Dto = dto,
            UserCode = userCode,
        });
        return result;
    }

    /// <summary>
    /// Update ControlCode
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpPost("updateControlCode")]
    public async Task<ControlCode> UpdateControlCode([FromBody] UpdateControlCodeDto dto, string userCode)
    {
        var result = await Mediator.Send(new UpdateCommand<UpdateControlCodeDto, ControlCode>()
        {
            Key = new string[] { dto.Code },
            Updated = dto,
            EntityType = EntityType.ControlCode,
            UserCode = userCode,
        });
        return result;
    }

    #endregion

    #region Uom

    /// <summary>
    /// Get Uom list
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpGet("getUomList")]
    public async Task<IEnumerable<UomListItemDto>> GetUomList([FromQuery] GetRegistrationListParameters parameters)
    {
        var gridFilter = mapper.Map<GetUomListDtoFilter>(parameters);
        var result = await Mediator.Send(new GetListQuery<GetUomListDtoFilter, UomListItemDto>()
        {
            Filter = gridFilter,
            EntityType = EntityType.Uom,
            Sorting = parameters.Sorting,
            Pagination = null,
        });
        return result.Items;
    }

    /// <summary>
    /// Get Uom
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpGet("getUom")]
    public async Task<Uom> GetUom(string code)
    {
        var result = await Mediator.Send(new GetQuery<Uom>()
        {
            Key = new string[] { code },
            EntityType = EntityType.Uom,
        });
        return result;
    }

    /// <summary>
    /// Add Uom
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpPost("addUom")]
    public async Task<Uom> AddUom([FromBody] AddUomDto dto, string userCode)
    {
        var result = await Mediator.Send(new AddUomCommand()
        {
            Dto = dto,
            UserCode = userCode,
        });
        return result;
    }

    /// <summary>
    /// Update Uom
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpPost("updateUom")]
    public async Task<Uom> UpdateUom([FromBody] UpdateUomDto dto, string userCode)
    {
        var result = await Mediator.Send(new UpdateCommand<UpdateUomDto, Uom>()
        {
            Key = new string[] { dto.Code },
            Updated = dto,
            EntityType = EntityType.Uom,
            UserCode = userCode,
        });
        return result;
    }

    #endregion

    #region Warehouse

    /// <summary>
    /// Get Warehouse list
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpGet("getWarehouseList")]
    public async Task<IEnumerable<WarehouseListItemDto>> GetWarehouseList([FromQuery] GetRegistrationListParameters parameters)
    {
        var gridFilter = mapper.Map<GetWarehouseListDtoFilter>(parameters);
        var result = await Mediator.Send(new GetListQuery<GetWarehouseListDtoFilter, WarehouseListItemDto>()
        {
            Filter = gridFilter,
            EntityType = EntityType.Warehouse,
            Sorting = parameters.Sorting,
            Pagination = null,
        });
        return result.Items;
    }

    /// <summary>
    /// Get Warehouse
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpGet("getWarehouse")]
    public async Task<Warehouse> GetWarehouse(string code)
    {
        var result = await Mediator.Send(new GetQuery<Warehouse>()
        {
            Key = new string[] { code },
            EntityType = EntityType.Warehouse,
        });
        return result;
    }

    /// <summary>
    /// Add Warehouse
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpPost("addWarehouse")]
    public async Task<Warehouse> AddWarehouse([FromBody] AddWarehouseDto dto, string userCode)
    {
        var result = await Mediator.Send(new AddCommand<AddWarehouseDto, Warehouse>()
        {
            Key = new string[] { dto.Code },
            Dto = dto,
            EntityType = EntityType.Warehouse,
            UserCode = userCode,
        });
        return result;
    }

    /// <summary>
    /// Update Warehouse
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpPost("updateWarehouse")]
    public async Task<Warehouse> UpdateWarehouse([FromBody] UpdateWarehouseDto dto, string userCode)
    {
        var result = await Mediator.Send(new UpdateCommand<UpdateWarehouseDto, Warehouse>()
        {
            Key = new string[] { dto.Code },
            Updated = dto,
            EntityType = EntityType.Warehouse,
            UserCode = userCode,
        });
        return result;
    }

    #endregion

    #region AreaType

    /// <summary>
    /// Get AreaType list
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpGet("getAreaTypeList")]
    public async Task<IEnumerable<AreaTypeListItemDto>> GetAreaTypeList([FromQuery] GetRegistrationListParameters parameters)
    {
        var gridFilter = mapper.Map<GetAreaTypeListDtoFilter>(parameters);
        var result = await Mediator.Send(new GetListQuery<GetAreaTypeListDtoFilter, AreaTypeListItemDto>()
        {
            Filter = gridFilter,
            EntityType = EntityType.AreaType,
            Sorting = parameters.Sorting,
            Pagination = null,
        });
        return result.Items;
    }

    /// <summary>
    /// Get AreaType
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpGet("getAreaType")]
    public async Task<AreaType> GetAreaType(string code)
    {
        var result = await Mediator.Send(new GetQuery<AreaType>()
        {
            Key = new string[] { code },
            EntityType = EntityType.AreaType,
        });
        return result;
    }

    /// <summary>
    /// Add AreaType
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpPost("addAreaType")]
    public async Task<AreaType> AddAreaType([FromBody] AddAreaTypeDto dto, string userCode)
    {
        var result = await Mediator.Send(new AddCommand<AddAreaTypeDto, AreaType>()
        {
            Key = new string[] { dto.Code },
            Dto = dto,
            EntityType = EntityType.AreaType,
            UserCode = userCode,
        });
        return result;
    }

    /// <summary>
    /// Update AreaType
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpPost("updateAreaType")]
    public async Task<AreaType> UpdateAreaType([FromBody] UpdateAreaTypeDto dto, string userCode)
    {
        var result = await Mediator.Send(new UpdateCommand<UpdateAreaTypeDto, AreaType>()
        {
            Key = new string[] { dto.Code },
            Updated = dto,
            EntityType = EntityType.AreaType,
            UserCode = userCode,
        });
        return result;
    }

    #endregion

    #region Area

    /// <summary>
    /// Get Area list
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpGet("getAreaList")]
    public async Task<IEnumerable<AreaListItemDto>> GetAreaList([FromQuery] GetAreaListParameters parameters)
    {
        var gridFilter = mapper.Map<GetAreaListDtoFilter>(parameters);
        var result = await Mediator.Send(new GetListQuery<GetAreaListDtoFilter, AreaListItemDto>()
        {
            Filter = gridFilter,
            EntityType = EntityType.AreaItem,
            Sorting = parameters.Sorting,
            Pagination = null,
        });
        return result.Items;
    }

    /// <summary>
    /// Get Area
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpGet("getArea")]
    public async Task<Area> GetArea(string code, string areaWhsCode)
    {
        var result = await Mediator.Send(new GetQuery<Area>()
        {
            Key = new string[] { code, areaWhsCode },
            EntityType = EntityType.Area,
        });
        return result;
    }

    /// <summary>
    /// Add Area
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpPost("addArea")]
    public async Task<Area> AddArea([FromBody] AddAreaDto dto, string userCode)
    {
        var result = await Mediator.Send(new AddCommand<AddAreaDto, Area>()
        {
            Key = new string[] { dto.Code, dto.WhsCode },
            Dto = dto,
            EntityType = EntityType.Area,
            UserCode = userCode,
        });
        return result;
    }

    /// <summary>
    /// Update Area
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpPost("updateArea")]
    public async Task<Area> UpdateArea([FromBody] UpdateAreaDto dto, string userCode)
    {
        var result = await Mediator.Send(new UpdateCommand<UpdateAreaDto, Area>()
        {
            Key = new string[] { dto.Code, dto.WhsCode },
            Updated = dto,
            EntityType = EntityType.Area,
            UserCode = userCode,
        });
        return result;
    }

    #endregion

    #region Location

    /// <summary>
    /// Get Location list
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpGet("getLocationList")]
    public async Task<PaginatedList<LocationListItemDto>> GetLocationList([FromQuery] GetLocationListParameters parameters)
    {
        var gridFilter = mapper.Map<GetLocationListDtoFilter>(parameters);
        var result = await Mediator.Send(new GetListQuery<GetLocationListDtoFilter, LocationListItemDto>()
        {
            Filter = gridFilter,
            EntityType = EntityType.Location,
            Sorting = parameters.Sorting,
            Pagination = parameters.Pagination,
        });
        return result;
    }

    /// <summary>
    /// Get Location
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpGet("getLocation")]
    public async Task<LocationDto> GetLocation(string code, string warehouseCode)
    {
        var result = await Mediator.Send(new GetQuery<LocationDto>()
        {
            Key = new string[] { code, warehouseCode },
            EntityType = EntityType.Location,
        });
        return result;
    }

    /// <summary>
    /// Add Location
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpPost("addLocation")]
    public async Task<LocationDto> AddLocation([FromBody] AddLocationDto dto, string userCode)
    {
        var result = await Mediator.Send(new AddCommand<AddLocationDto, LocationDto>()
        {
            Key = new string[] { dto.Code, dto.WarehouseCode },
            Dto = dto,
            EntityType = EntityType.Location,
            UserCode = userCode,
        });
        return result;
    }

    /// <summary>
    /// Update Location
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpPost("updateLocation")]
    public async Task<LocationDto> UpdateLocation([FromBody] UpdateLocationDto dto, string userCode)
    {
        var result = await Mediator.Send(new UpdateLocationCommand()
        {
            Updated = dto,
            UserCode = userCode,
        });
        return result;
    }

    /// <summary>
    /// Toggle active Location
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpPost("toggleActiveLocation")]
    public async Task<LocationDto> ToggleActiveLocation(string code, string warehouseCode, string userCode)
    {
        var result = await Mediator.Send(new ToggleActiveLocationCommand()
        {
            Code = code,
            WarehouseCode = warehouseCode,
            UserCode = userCode,
        });
        return result;
    }

    #endregion

    #region LabelPrinter

    /// <summary>
    /// Get LabelPrinter list
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpGet("getLabelPrinterList")]
    public async Task<IEnumerable<LabelPrinterListItemDto>> GetLabelPrinterList([FromQuery] GetLabelPrinterListParameters parameters)
    {
        var gridFilter = mapper.Map<GetLabelPrinterListDtoFilter>(parameters);
        var result = await Mediator.Send(new GetListQuery<GetLabelPrinterListDtoFilter, LabelPrinterListItemDto>()
        {
            Filter = gridFilter,
            EntityType = EntityType.LabelPrinter,
            Sorting = parameters.Sorting,
            Pagination = null,
        });
        return result.Items;
    }

    /// <summary>
    /// Get LabelPrinter
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpGet("getLabelPrinter")]
    public async Task<LabelPrinter> GetLabelPrinter(string ip)
    {
        var result = await Mediator.Send(new GetQuery<LabelPrinter>()
        {
            Key = new string[] { ip },
            EntityType = EntityType.LabelPrinter,
        });
        return result;
    }

    /// <summary>
    /// Add LabelPrinter
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpPost("addLabelPrinter")]
    public async Task<LabelPrinter> AddLabelPrinter([FromBody] AddLabelPrinterDto dto, string userCode)
    {
        var result = await Mediator.Send(new AddCommand<AddLabelPrinterDto, LabelPrinter>()
        {
            Key = new string[] { dto.Ip },
            Dto = dto,
            EntityType = EntityType.LabelPrinter,
            UserCode = userCode,
        });
        return result;
    }

    /// <summary>
    /// Update LabelPrinter
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpPost("updateLabelPrinter")]
    public async Task<LabelPrinter> UpdateLabelPrinter([FromBody] UpdateLabelPrinterDto dto, string userCode)
    {
        var result = await Mediator.Send(new UpdateCommand<UpdateLabelPrinterDto, LabelPrinter>()
        {
            Key = new string[] { dto.Ip },
            Updated = dto,
            EntityType = EntityType.LabelPrinter,
            UserCode = userCode,
        });
        return result;
    }

    /// <summary>
    /// Delete LabelPrinter
    /// </summary>
    [FeatureGate(FeatureFlags.Registration)]
    [HttpDelete("deleteLabelPrinter")]
    public async Task<ActionResult> DeleteLabelPrinter(string ip, string userCode)
    {
        var result = await Mediator.Send(new DeleteLabelPrinterCommand()
        {
            IP = ip,
            UserCode = userCode,
        });
        return Ok();
    }

    #endregion

}


