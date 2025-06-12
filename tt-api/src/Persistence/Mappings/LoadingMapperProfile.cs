using AutoMapper;
using Domain.Entities;
using Persistence.Entities;

namespace Persistence.Mappings;

internal class LoadingMapperProfile : Profile
{
    public LoadingMapperProfile()
    {
        CreateMap<TtLoading, Loading>()
            .ForMember(s => s.WarehouseCode, db => db.MapFrom(i => i.Whscode));
    }
}
