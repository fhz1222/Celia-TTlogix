using Application.Common.Models;
using Application.UseCases.CompanyProfiles.Queries.GetCompanyProfiles;
using AutoMapper;

namespace Presentation.Common;

/// <summary>
/// This class is used by CompanyProfileController to specify required parameters, filters, sorting and pagination requirements
/// for the Company Profile list.
/// </summary>
public class CompanyProfilesParameters
{
    /// <summary>
    /// Company Profile Code filter
    /// </summary>
    public string? Code { get; set; } = null!;

    /// <summary>
    /// Company Profile Name filter
    /// </summary>
    public string? Name { get; set; } = null!;

    /// <summary>
    /// Company Profile status filter
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// Specifies the field used for sorting and if it is ascending (default) or descending  
    /// </summary>  
    public OrderBy? Sorting { get; set; }

    /// <summary>
    /// This class creates a mapping from CompanyProfileParameters to GetCompanyProfilesDtoFilter.
    /// </summary>
    public class MapperProfile : Profile
    {
        /// <summary>
        /// Class constructor.
        /// </summary>
        public MapperProfile() => CreateMap<CompanyProfilesParameters, GetCompanyProfilesDtoFilter>();
    }
}



