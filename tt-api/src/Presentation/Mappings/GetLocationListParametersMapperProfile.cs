using Application.UseCases.Registration.Queries.GetLocationList;
using AutoMapper;
using Presentation.Common;

namespace Presentation.Mappings
{
    /// <summary>
    /// Automapper for registration list parameters
    /// </summary>
    public class GetLocationListParametersMapperProfile : Profile
    {
        /// <summary>
        /// default constructor - creates the map
        /// </summary>
        public GetLocationListParametersMapperProfile()
        {
            CreateMap<GetLocationListParameters, GetLocationListDtoFilter>()
                .ForMember(x => x.WhsCode, y => y.MapFrom(z => z.WarehouseCode));
        }
    }
}
