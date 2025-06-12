using Application.UseCases.Quarantine.Queries.GetQuarantineItems;
using AutoMapper;
using Presentation.Common;

namespace Persistence.Mappings
{
    /// <summary>
    /// Automapper for quarantine data parameters; maps QuarantineParameters object to QuarantineItemDtoFilter object
    /// </summary>
    public class QuarantineParametersMapperProfile : Profile
    {
        /// <summary>
        /// default constructor - creates the map
        /// </summary>
        public QuarantineParametersMapperProfile()
        {
            CreateMap<QuarantineParameters, QuarantineItemDtoFilter>()
                .ForMember(s => s.CustomerCode, d => d.MapFrom(i => i.CustomerCode))
                .ForMember(s => s.SupplierId, d => d.MapFrom(i => i.SupplierId))
                .ForMember(s => s.ProductCode, d => d.MapFrom(i => i.ProductCode))
                .ForMember(s => s.Location, d => d.MapFrom(i => i.Location))
                .ForMember(s => s.Reason, d => d.MapFrom(i => i.Reason))
                .ForMember(s => s.PID, d => d.MapFrom(i => i.PID))
                .ForMember(s => s.CreatedBy, d => d.MapFrom(i => i.CreatedBy))
                .ForMember(s => s.Qty, d => d.MapFrom(i => i.Qty));
        }
    }
}
