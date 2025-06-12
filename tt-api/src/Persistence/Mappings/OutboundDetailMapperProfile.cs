using AutoMapper;
using Domain.Entities;
using Domain.ValueObjects;
using Persistence.Entities;

namespace Persistence.Mappings;

internal class OutboundDetailMapperProfile : Profile
{
    public OutboundDetailMapperProfile()
    {
        CreateMap<TtOutboundDetail, OutboundItem>()
            .ForMember(s => s.ItemNo, db => db.MapFrom(i => i.LineItem))
            .ForMember(s => s.Status, db => db.MapFrom(i => OutboundDetailStatus.From(i.Status)))
            .ForMember(s => s.Qty, db => db.MapFrom(i => (int)i.Qty))
            .ForMember(s => s.PickedQty, db => db.MapFrom(i => (int)i.PickedQty));
    }
}
