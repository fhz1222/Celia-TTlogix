using Application.UseCases.Quarantine;
using AutoMapper;
using Persistence.PetaPoco.Models;

namespace Persistence.Mappings
{
    public class QuarantineMapperProfile : Profile
    {
        public QuarantineMapperProfile()
        {
            CreateMap<TT_StorageDetail, QuarantineItemDto>()
                .ForMember(s => s.PID, db => db.MapFrom(i => i.PID))
                .ForMember(s => s.ProductCode, db => db.MapFrom(i => i.ProductCode))
                .ForMember(s => s.CustomerCode, db => db.MapFrom(i => i.CustomerCode))
                .ForMember(s => s.SupplierId, db => db.MapFrom(i => i.SupplierId))
                .ForMember(s => s.Qty, db => db.MapFrom(i => (int) i.Qty))
                .ForMember(s => s.Location, db => db.MapFrom(i => i.LocationCode))
                .ForMember(s => s.WhsCode, db => db.MapFrom(i => i.WHSCode));

            CreateMap<TT_QuarantineLog, QuarantineItemDto>()
                .ForMember(s => s.CreatedBy, db => db.MapFrom(i => i.CreatedBy))
                .ForMember(s => s.QuarantineDate, db => db.MapFrom(i => i.CreatedDate))
                .ForMember(s => s.PID, db => db.Ignore()) ;
        }
    }
}
