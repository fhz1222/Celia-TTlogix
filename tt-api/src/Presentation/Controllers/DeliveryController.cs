using Application.UseCases.Common.Queries.GetListQuery;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Application.UseCases.Common;
using Application.UseCases.Delivery.Queries.GetDeliveryCustomerClientList;

namespace Presentation.Controllers;

/// <summary>
/// DeliveryController
/// </summary>
public partial class DeliveryController : ApiControllerBase
{
    private readonly IMapper mapper;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="mapper"></param>
    public DeliveryController(IMapper mapper)
    {
        this.mapper = mapper;
    }

    /// <summary>
    /// Gets active customer clients for unitec report
    /// </summary>
    [HttpGet("getDeliveryCustomerClients")]
    public async Task<IEnumerable<DeliveryCustomerClientDto>> GetDeliveryCustomerClients(string customerCode)
    {
        var result = await Mediator.Send(new GetListQuery<GetDeliveryCustomerClientListFilter, DeliveryCustomerClientDto>()
        {
            Filter = new GetDeliveryCustomerClientListFilter {
                Status = Status.Active,
                CustomerCode = customerCode
            },
            EntityType = EntityType.DeliveryCustomerClient,
            Sorting = null,
            Pagination = null,
        });
        return result.Items;
    }       
}


