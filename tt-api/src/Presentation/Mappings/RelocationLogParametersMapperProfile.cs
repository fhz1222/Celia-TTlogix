using Application.UseCases;
using Application.UseCases.RelocationLogs.Queries.GetRelocationLogs;
using AutoMapper;
using Domain.ValueObjects;
using Presentation.Common;

namespace Persistence.Mappings
{
    /// <summary>
    /// Automapper for relocation log parameters; maps RelocationLogParameters object to RelocationLogDtoFilter object
    /// </summary>
    public class RelocationLogParametersMapperProfile : Profile
    {
        /// <summary>
        /// default constructor - creates the map
        /// </summary>
        public RelocationLogParametersMapperProfile()
        {
            CreateMap<RelocationLogParameters, RelocationLogDtoFilter>()
                .ForMember(s => s.PID, d => d.MapFrom(i => i.PID))
                .ForMember(s => s.ExternalPID, d => d.MapFrom(i => i.ExternalPID))
                .ForMember(s => s.SupplierId, d => d.MapFrom(i => i.SupplierId))
                .ForMember(s => s.ProductCode, d => d.MapFrom(i => i.ProductCode))
                .ForMember(s => s.OldWhsCode, d => d.MapFrom(i => i.OldWhsCode))
                .ForMember(s => s.OldLocation, d => d.MapFrom(i => i.OldLocation))
                .ForMember(s => s.NewWhsCode, d => d.MapFrom(i => i.NewWhsCode))
                .ForMember(s => s.NewLocation, d => d.MapFrom(i => i.NewLocation))
                .ForMember(s => s.ScannerType, d => d.MapFrom(i => i.ScannerType.HasValue ? ScannerType.From(i.ScannerType.Value) : null))
                .ForMember(s => s.RelocatedBy, d => d.MapFrom(i => i.RelocatedBy))
                .ForMember(s => s.CustomerCode, d => d.MapFrom(i => i.CustomerCode))
                .ForMember(s => s.RelocationDate, db => db.MapFrom(i => i.RelocationDate != null ? i.RelocationDate : new DtoFilterDateTimeRange()))
                .ForMember(s => s.Qty, d => d.MapFrom(i => i.Qty));
        }
    }
}
