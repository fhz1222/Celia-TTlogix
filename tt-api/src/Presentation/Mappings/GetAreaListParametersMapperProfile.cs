using Application.UseCases.Registration.Queries.GetAreaList;
using AutoMapper;
using Presentation.Common;

namespace Presentation.Mappings
{
    /// <summary>
    /// Automapper for registration list parameters
    /// </summary>
    public class GetAreaListParametersMapperProfile : Profile
    {
        /// <summary>
        /// default constructor - creates the map
        /// </summary>
        public GetAreaListParametersMapperProfile()
        {
            CreateMap<GetAreaListParameters, GetAreaListDtoFilter>();
        }
    }
}
