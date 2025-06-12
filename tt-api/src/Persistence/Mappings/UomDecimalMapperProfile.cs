using AutoMapper;
using Domain.Entities;
using Domain.ValueObjects;
using Persistence.Entities;

namespace Persistence.Mappings
{
    public class UomDecimalMapperProfile : Profile
    {
        public UomDecimalMapperProfile()
        {
            CreateMap<TtUOMDecimal, UomDecimal>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (UomDecimalStatus)c.Status));
            
            CreateMap<UomDecimal, TtUOMDecimal>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (byte)c.Status));

        }
    }
}
