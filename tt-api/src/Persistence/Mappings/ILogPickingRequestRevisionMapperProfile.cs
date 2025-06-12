using AutoMapper;
using Domain.Entities;
using Persistence.Entities;

namespace Persistence.Mappings;

public class ILogPickingRequestRevisionMapperProfile : Profile
{
    public ILogPickingRequestRevisionMapperProfile()
    {
        CreateMap<ILogPickingRequestRevision, PickingRequestRevision>();
        CreateMap<PickingRequestRevision, ILogPickingRequestRevision>();
    }
}