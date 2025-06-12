using Application.UseCases.Decants.Queries.GetDecants;
using AutoMapper;
using Domain.ValueObjects;
using Presentation.Common;

namespace Persistence.Mappings
{
    /// <summary>
    /// Automapper for decant parameters; maps DecantParameters object to DecantDtoFilter object
    /// </summary>
    public class DecantParametersMapperProfile : Profile
    {
        /// <summary>
        /// default constructor - creates the map
        /// </summary>
        public DecantParametersMapperProfile()
        {
            CreateMap<DecantParameters, DecantDtoFilter>()
                .ForMember(s => s.CustomerCode, d => d.MapFrom(i => i.CustomerCode))
                .ForMember(s => s.JobNo, d => d.MapFrom(i => i.JobNo))
                .ForMember(s => s.ReferenceNo, d => d.MapFrom(i => i.ReferenceNo))
                .ForMember(s => s.Status, d => d.MapFrom(i => i.Status))
                .ForMember(s => s.Remark, d => d.MapFrom(i => i.Remark))
                .ForMember(s => s.CreatedDate, d => d.MapFrom(i => i.CreatedDate));
        }
    }
}
