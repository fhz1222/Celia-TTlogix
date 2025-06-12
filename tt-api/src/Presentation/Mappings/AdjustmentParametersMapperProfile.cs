using Application.UseCases.Adjustments.Queries.GetAdjustmentsQuery;
using AutoMapper;
using Domain.ValueObjects;
using Presentation.Common;

namespace Persistence.Mappings
{
    /// <summary>
    /// Automapper for adjustment parameters; maps AdjustmentParameters object to CustomerAdjustmentFilter object
    /// </summary>
    public class AdjustmentParametersMapperProfile : Profile
    {
        /// <summary>
        /// default constructor - creates the map
        /// </summary>
        public AdjustmentParametersMapperProfile()
        {
            CreateMap<AdjustmentParameters, AdjustmentFilter>()
                .ForMember(s => s.CustomerCode, d => d.MapFrom(i => i.CustomerCode))
                .ForMember(s => s.JobNo, d => d.MapFrom(i => i.JobNo))
                .ForMember(s => s.ReferenceNo, d => d.MapFrom(i => i.ReferenceNo))
                .ForMember(s => s.Status, d => d.MapFrom(i => i.Status))
                .ForMember(s => s.Reason, d => d.MapFrom(i => i.Reason))
                .ForMember(s => s.JobType, d => d.MapFrom(i => i.JobType.HasValue ? InventoryAdjustmentJobType.From(i.JobType.Value) : null))
                .ForMember(s => s.CreatedDate, d => d.MapFrom(i => i.CreatedDate));
        }
    }
}
