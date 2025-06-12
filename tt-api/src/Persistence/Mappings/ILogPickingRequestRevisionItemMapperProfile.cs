using AutoMapper;
using Domain.Entities;
using Persistence.Entities;

namespace Persistence.Mappings;

public class ILogPickingRequestRevisionItemMapperProfile : Profile
{
    public ILogPickingRequestRevisionItemMapperProfile()
    {
        CreateMap<ILogPickingRequestRevisionItem, PickingRequestRevisionItem>()
            .ForMember(i => i.PalletId, db => db.MapFrom(i => i.Pid));
    }
}