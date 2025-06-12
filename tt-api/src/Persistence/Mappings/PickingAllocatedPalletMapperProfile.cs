using AutoMapper;
using Domain.Entities;
using Persistence.Entities;

namespace Persistence.Mappings;

public class PickingAllocatedPalletMapperProfile : Profile
{
    public PickingAllocatedPalletMapperProfile()
    {
        CreateMap<TtPickingAllocatedPid, PickingAllocatedPallet>()
            .ForMember(s => s.LineNo, db => db.MapFrom(i => i.LineItem))
            .ForMember(s => s.SeqNo, db => db.MapFrom(i => i.SerialNo))
            .ForMember(s => s.AllocatedQty, db => db.MapFrom(i => (int?)i.AllocatedQty))
            .ForMember(s => s.PickedQty, db => db.MapFrom(i => (int?)i.PickedQty));
    }
}
