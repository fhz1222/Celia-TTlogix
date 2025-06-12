using AutoMapper;
using Domain.Entities;
using Persistence.Entities;

namespace Persistence.Mappings;

public class PickingListMapperProfile : Profile
{
    public PickingListMapperProfile()
    {
        CreateMap<TtPickingList, PickingListItem>()
            .ForMember(s => s.LineNo, db => db.MapFrom(i => i.LineItem))
            .ForMember(s => s.PalletInboundDate, db => db.MapFrom(i => i.InboundDate))
            .ForMember(s => s.PalletInboundJobNo, db => db.MapFrom(i => i.InboundJobNo))
            .ForMember(s => s.Whs, db => db.MapFrom(i => i.Whscode))
            .ForMember(s => s.Qty, db => db.MapFrom(i => (int)i.Qty));
    }
}
