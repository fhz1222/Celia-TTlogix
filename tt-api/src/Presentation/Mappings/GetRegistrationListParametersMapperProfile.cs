using Application.UseCases.Registration.Queries.GetControlCodeList;
using Application.UseCases.Registration.Queries.GetPackageTypeList;
using Application.UseCases.Registration.Queries.GetProductCodeList;
using Application.UseCases.Registration.Queries.GetUomList;
using Application.UseCases.Registration.Queries.GetWarehouseList;
using Application.UseCases.Registration.Queries.GetAreaTypeList;
using AutoMapper;
using Presentation.Common;

namespace Presentation.Mappings
{
    /// <summary>
    /// Automapper for registration list parameters
    /// </summary>
    public class GetRegistrationListParametersMapperProfile : Profile
    {
        /// <summary>
        /// default constructor - creates the map
        /// </summary>
        public GetRegistrationListParametersMapperProfile()
        {
            CreateMap<GetRegistrationListParameters, GetPackageTypeListDtoFilter>();
            CreateMap<GetRegistrationListParameters, GetProductCodeListDtoFilter>();
            CreateMap<GetRegistrationListParameters, GetControlCodeListDtoFilter>();
            CreateMap<GetRegistrationListParameters, GetUomListDtoFilter>();
            CreateMap<GetRegistrationListParameters, GetWarehouseListDtoFilter>();
            CreateMap<GetRegistrationListParameters, GetAreaTypeListDtoFilter>();
        }
    }
}
