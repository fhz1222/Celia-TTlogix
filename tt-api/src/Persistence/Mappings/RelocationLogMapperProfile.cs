using Application.UseCases.RelocationLogs;
using AutoMapper;
using Domain.Entities;
using Domain.ValueObjects;
using Persistence.Entities;
using Persistence.PetaPoco.Models;

namespace Persistence.Mappings
{
    public class RelocationLogMapperProfile : Profile
    {
        public RelocationLogMapperProfile()
        {
            CreateMap<TT_RelocationLog, RelocationLogDto>()
                .ForMember(s => s.PID, db => db.MapFrom(i => i.PID))
                .ForMember(s => s.ExternalPID, db => db.MapFrom(i => i.ExternalPID))
                .ForMember(s => s.OldWhsCode, db => db.MapFrom(i => i.OldWHSCode))
                .ForMember(s => s.NewWhsCode, db => db.MapFrom(i => i.NewWHSCode))
                .ForMember(s => s.OldLocation, db => db.MapFrom(i => i.OldLocationCode))
                .ForMember(s => s.NewLocation, db => db.MapFrom(i => i.NewLocationCode))
                .ForMember(s => s.ScannerType, db => db.MapFrom(i => ScannerType.From(i.ScannerType)))
                .ForMember(s => s.RelocatedBy, db => db.MapFrom(i => i.RelocatedBy))
                .ForMember(s => s.RelocationDate, db => db.MapFrom(i => i.RelocatedDate))
                ;

            CreateMap<RelocationLog, TtRelocationLog>()
                .ForMember(s => s.PID, db => db.MapFrom(i => i.PalletId))
                .ForMember(s => s.ExternalPID, db => db.MapFrom(i => i.ExternalPalletId))
                .ForMember(s => s.NewWHSCode, db => db.MapFrom(i => i.TargetLocation.WarehouseCode))
                .ForMember(s => s.NewLocationCode, db => db.MapFrom(i => i.TargetLocation.Code))
                .ForMember(s => s.OldWHSCode, db => db.MapFrom(i => i.SourceLocation.WarehouseCode))
                .ForMember(s => s.OldLocationCode, db => db.MapFrom(i => i.SourceLocation.Code))
                .ForMember(s => s.ScannerType, db => db.MapFrom(i => i.ScannerType))
                .ForMember(s => s.RelocatedBy, db => db.MapFrom(i => i.RelocatedBy))
                .ForMember(s => s.RelocatedDate, db => db.MapFrom(i => i.RelocatedOn));
        }
    }
}
