using Application.Common.Models;
using Application.UseCases;
using Application.UseCases.Customer.Queries.GetUomDecimalList;
using AutoMapper;

namespace Presentation.Common;

/// <summary>
/// This class is used by CustomerController to specify required parameters, filters, sorting and pagination requirements
/// for the uom decimal list.
/// </summary>
public class UomDecimalListParameters
{
    /// <summary>
    /// Customer code
    /// </summary>
    public string CustomerCode { get; set; } = null!;

    /// <summary>
    /// Uom decimal name filter
    /// </summary>
    public string? Name { get; set; } = null!;

    /// <summary>
    /// Uom decimal status filter
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// Uom decimal filter
    /// </summary>
    public DtoFilterIntRange? DecimalNum { get; set; }

    /// <summary>
    /// Specifies the field used for sorting and if it is ascending (default) or descending  
    /// </summary>  
    public OrderBy? Sorting { get; set; }

    /// <summary>
    /// This class creates a mapping from UomListParameters to GetUomListDtoFilter.
    /// </summary>
    public class MapperProfile : Profile
    {
        /// <summary>
        /// Class constructor.
        /// </summary>
        public MapperProfile() => CreateMap<UomDecimalListParameters, GetUomDecimalListDtoFilter>();
    }
}



