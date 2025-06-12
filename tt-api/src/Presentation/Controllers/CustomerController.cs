using Application.UseCases.Customer;
using Application.UseCases.Customer.Queries.GetCustomers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using Presentation.Common;
using Presentation.Configuration;
using Application.UseCases.Customer.Queries.GetCustomerList;
using Domain.Entities;
using Application.UseCases.Customer.Commands.UpdateCustomerCommand;
using Application.UseCases.Customer.Commands.ToggleActiveCommand;
using Application.UseCases.Customer.Queries.GetInventoryControl;
using Application.UseCases.Customer.Commands.UpdateInventoryControlCommand;
using Application.UseCases.Customer.Queries.GetUomDecimalList;
using Application.UseCases.Customer.Commands.ToggleActiveUomDecimalCommand;
using Application.UseCases.Customer.Commands.UpdateUomDecimalCommand;
using Application.UseCases.Customer.Queries.GetCustomerClientList;
using Application.UseCases.Customer.Commands.UpdateCustomerClientCommand;
using Application.UseCases.Customer.Commands.ToggleActiveCustomerClientCommand;
using Application.UseCases.Common.Queries.GetListQuery;
using Application.UseCases.CompanyProfiles;
using Domain.ValueObjects;
using Application.UseCases.Common;
using Application.UseCases.Common.Commands.UpsertCommand;

namespace Presentation.Controllers;
/// <summary>
/// InventoryControler provides method to get the storage detail list
/// </summary>
public partial class CustomerController : ApiControllerBase
{
    private readonly IMapper mapper;
    private readonly IFeatureManager manager;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="manager"></param>
    public CustomerController(IMapper mapper, IFeatureManager manager)
    {
        this.mapper = mapper;
        this.manager = manager;
    }

    [HttpGet("isActive")]
    public async Task<bool> IsActive()
        => await manager.IsEnabledAsync(FeatureFlags.Customers);

    /// <summary>
    /// Gets customers
    /// </summary>
    [HttpGet]
    [Produces("application/json")]
    public async Task<IEnumerable<CustomerDto>> GetCustomers()
    {
        return await Mediator.Send(new GetCustomersQuery());
    }

    /// <summary>
    /// Gets customer
    /// </summary>
    [FeatureGate(FeatureFlags.Customers)]
    [HttpGet("getCustomer")]
    public async Task<Customer> GetCustomer(string code, string whsCode)
    {
        return await Mediator.Send(new GetQuery<Customer>()
        {
            Key = new string[] { code, whsCode },
            EntityType = EntityType.Customer
        });
    }

    /// <summary>
    /// Gets customers for customer main list
    /// </summary>
    [FeatureGate(FeatureFlags.Customers)]
    [HttpGet("getCustomerList")]
    public async Task<IEnumerable<CustomerListItemDto>> GetCustomerList([FromQuery] CustomerListParameters parameters)
    {
        var gridFilter = mapper.Map<GetCustomerListDtoFilter>(parameters);
        return await Mediator.Send(new GetCustomerListQuery()
        {
            Filter = gridFilter,
            OrderByExpression = parameters.Sorting?.By,
            OrderByDescending = parameters.Sorting?.Descending ?? true
        });
    }

    /// <summary>
    /// Create new customer or update existing
    /// </summary>
    [FeatureGate(FeatureFlags.Customers)]
    [HttpPost("update")]
    public async Task<Customer> Update(UpdateCustomerDto customer, string userCode)
    {
        return await Mediator.Send(new UpdateCustomerCommand()
        {
            Customer = customer,
            UserCode = userCode
        });
    }

    /// <summary>
    /// Activate or deactivate customer
    /// </summary>
    [FeatureGate(FeatureFlags.Customers)]
    [HttpPost("toggleActive")]
    public async Task<Customer> ToggleActive(string code, string whsCode, string userCode)
    {
        return await Mediator.Send(new ToggleActiveCommand()
        {
            Code = code,
            WhsCode = whsCode,
            UserCode = userCode
        });
    }

    /// <summary>
    /// Gets customer inventory ctrl
    /// </summary>
    [FeatureGate(FeatureFlags.Customers)]
    [HttpGet("getInventoryControl")]
    public async Task<InventoryControlDto> GetInventoryControl(string customerCode)
    {
        return await Mediator.Send(new GetInventoryControlQuery()
        {
            CustomerCode = customerCode,
        });
    }

    /// <summary>
    /// Create new customer inventory ctrl or update existing
    /// </summary>
    [FeatureGate(FeatureFlags.Customers)]
    [HttpPost("updateInventoryControl")]
    public async Task<InventoryControl> UpdateInventoryControl(InventoryControlDto inventoryControl, string userCode)
    {
        return await Mediator.Send(new UpdateInventoryControlCommand()
        {
            InventoryControl = inventoryControl,
            UserCode = userCode,
        });
    }

    /// <summary>
    /// Gets UOM list
    /// </summary>
    [FeatureGate(FeatureFlags.Customers)]
    [HttpGet("getActiveUomList")]
    public async Task<IEnumerable<UomDto>> GetActiveUomList()
    {
        var result = await Mediator.Send(new GetListQuery<UomFilter, UomDto>()
        {
            Filter = new UomFilter { Status = Status.Active },
            EntityType = EntityType.Uom,
            Sorting = null,
            Pagination = null,
        });
        return result.Items;
    }

    /// <summary>
    /// Gets UOM decimal main list
    /// </summary>
    [FeatureGate(FeatureFlags.Customers)]
    [HttpGet("getUomDecimalList")]
    public async Task<IEnumerable<UomDecimalListItemDto>> GetUomDecimalList([FromQuery] UomDecimalListParameters parameters)
    {
        var gridFilter = mapper.Map<GetUomDecimalListDtoFilter>(parameters);
        return await Mediator.Send(new GetUomDecimalListQuery()
        {
            Filter = gridFilter,
            OrderBy = parameters.Sorting?.By,
            OrderByDescending = parameters.Sorting?.Descending ?? false
        });
    }

    /// <summary>
    /// Create new UOM decimal or update existing
    /// </summary>
    [FeatureGate(FeatureFlags.Customers)]
    [HttpPost("updateUomDecimal")]
    public async Task<UomDecimal> UpdateUomDecimal(UomDecimalDto uomDecimal, string userCode)
    {
        return await Mediator.Send(new UpdateUomDecimalCommand()
        {
            UomDecimal = uomDecimal,
            UserCode = userCode
        });
    }

    /// <summary>
    /// Activate or deactivate UOM Decimal
    /// </summary>
    [FeatureGate(FeatureFlags.Customers)]
    [HttpPost("toggleActiveUomDecimal")]
    public async Task<UomDecimal> ToggleActiveUomDecimal(string code, string customerCode, string userCode)
    {
        return await Mediator.Send(new ToggleActiveUomDecimalCommand()
        {
            Code = code,
            CustomerCode = customerCode,
            UserCode = userCode
        });
    }

    /// <summary>
    /// Gets CustomerClient main list
    /// </summary>
    [FeatureGate(FeatureFlags.Customers)]
    [HttpGet("getCustomerClientList")]
    public async Task<IEnumerable<CustomerClientListItemDto>> GetCustomerClientList([FromQuery] CustomerClientListParameters parameters)
    {
        var gridFilter = mapper.Map<GetCustomerClientListDtoFilter>(parameters);
        return await Mediator.Send(new GetCustomerClientListQuery()
        {
            Filter = gridFilter,
            OrderBy = parameters.Sorting?.By,
            OrderByDescending = parameters.Sorting?.Descending ?? false
        });
    }

    /// <summary>
    /// Gets customer client
    /// </summary>
    [FeatureGate(FeatureFlags.Customers)]
    [HttpGet("getCustomerClient")]
    public async Task<CustomerClient> GetCustomerClient(string code)
    {
        return await Mediator.Send(new GetQuery<CustomerClient>()
        {
            Key = new string[] { code },
            EntityType = EntityType.CustomerClient
        });
    }

    /// <summary>
    /// Create new CustomerClient or update existing
    /// </summary>
    [FeatureGate(FeatureFlags.Customers)]
    [HttpPost("updateCustomerClient")]
    public async Task<CustomerClient> UpdateCustomerClient(CustomerClientDto CustomerClient, string userCode)
    {
        return await Mediator.Send(new UpdateCustomerClientCommand()
        {
            CustomerClient = CustomerClient,
            UserCode = userCode
        });
    }

    /// <summary>
    /// Activate or deactivate CustomerClient
    /// </summary>
    [FeatureGate(FeatureFlags.Customers)]
    [HttpPost("toggleActiveCustomerClient")]
    public async Task<CustomerClient> ToggleActiveCustomerClient(string code, string userCode)
    {
        return await Mediator.Send(new ToggleActiveCustomerClientCommand()
        {
            Code = code,
            UserCode = userCode
        });
    }

    /// <summary>
    /// Gets active control codes
    /// </summary>
    [FeatureGate(FeatureFlags.Customers)]
    [HttpGet("getActiveControlCodes")]
    public async Task<IEnumerable<ControlCodeDto>> GetActiveControlCodes()
    {
        var result = await Mediator.Send(new GetListQuery<ControlCodeFilter, ControlCodeDto>()
        {
            Filter = new ControlCodeFilter { Status = Status.Active },
            EntityType = EntityType.ControlCode,
            Sorting = null,
            Pagination = null,
        });
        return result.Items;
    }

    /// <summary>
    /// Gets active product codes
    /// </summary>
    [FeatureGate(FeatureFlags.Customers)]
    [HttpGet("getActiveProductCodes")]
    public async Task<IEnumerable<ProductCodeDto>> GetActiveProductCodes()
    {
        var result = await Mediator.Send(new GetListQuery<ProductCodeFilter, ProductCodeDto>()
        {
            Filter = new ProductCodeFilter { Status = Status.Active },
            EntityType = EntityType.ProductCode,
            Sorting = null,
            Pagination = null,
        });
        return result.Items;
    }
}


