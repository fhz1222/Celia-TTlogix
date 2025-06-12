using Application.Common.Models;
using Application.UseCases.Customer.Queries.GetCustomerClientList;
using AutoMapper;
using Domain.ValueObjects;

namespace Presentation.Common;

/// <summary>
/// This class is used by CustomerController to specify required parameters, filters, sorting and pagination requirements
/// for the Customer Client list.
/// </summary>
public class CustomerClientListParameters
{
    /// <summary>
    /// Customer Code filter
    /// </summary>
    public string CustomerCode { get; set; } = null!;

    /// <summary>
    /// Code filter
    /// </summary>
    public string? Code { get; set; } = null!;

    /// <summary>
    /// Name filter
    /// </summary>
    public string? Name { get; set; } = null!;

    /// <summary>
    /// Contact person filter
    /// </summary>
    public string? ContactPerson { get; set; } = null!;

    /// <summary>
    /// Telephone number filter
    /// </summary>
    public string? TelephoneNo { get; set; } = null!;

    /// <summary>
    /// Fax number filter
    /// </summary>
    public string? FaxNo { get; set; } = null!;

    /// <summary>
    /// Client status filter
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// Specifies the field used for sorting and if it is ascending (default) or descending  
    /// </summary>  
    public OrderBy? Sorting { get; set; }

    /// <summary>
    /// This class creates a mapping from ClientListParameters to GetClientListDtoFilter.
    /// </summary>
    public class MapperProfile : Profile
    {
        /// <summary>
        /// Class constructor.
        /// </summary>
        public MapperProfile() => CreateMap<CustomerClientListParameters, GetCustomerClientListDtoFilter>();
    }
}



