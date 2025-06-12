using Application.Common.Models;
using Application.UseCases.Customer.Queries.GetCustomerList;
using AutoMapper;
using Domain.Enums;

namespace Presentation.Common;

/// <summary>
/// This class is used by CustomerController to specify required parameters, filters, sorting and pagination requirements
/// for the customer list.
/// </summary>
public class CustomerListParameters
{
    /// <summary>
    /// WHS code
    /// </summary>
    public string WhsCode { get; set; } = null!;

    /// <summary>
    /// Customer Code filter
    /// </summary>
    public string? Code { get; set; } = null!;

    /// <summary>
    /// Customer Name filter
    /// </summary>
    public string? Name { get; set; } = null!;

    /// <summary>
    /// Contact person filter
    /// </summary>
    public string? ContactPerson { get; set; } = null!;

    /// <summary>
    /// Telephone number filter
    /// </summary>
    public string? TelephoneNo { get; set; }

    /// <summary>
    /// Fax number filter
    /// </summary>
    public string? FaxNo { get; set; }

    /// <summary>
    /// Customer status filter
    /// </summary>
    public CustomerStatus? Status { get; set; }

    /// <summary>
    /// Specifies the field used for sorting and if it is ascending (default) or descending  
    /// </summary>  
    public OrderBy? Sorting { get; set; }

    /// <summary>
    /// This class creates a mapping from CustomerListParameters to GetCustomerListDtoFilter.
    /// </summary>
    public class MapperProfile : Profile
    {
        /// <summary>
        /// Class constructor.
        /// </summary>
        public MapperProfile() => CreateMap<CustomerListParameters, GetCustomerListDtoFilter>();
    }
}



