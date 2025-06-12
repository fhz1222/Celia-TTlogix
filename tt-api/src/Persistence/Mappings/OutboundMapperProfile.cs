using AutoMapper;
using Domain.Entities;
using Domain.ValueObjects;
using Persistence.Entities;

namespace Persistence.Mappings;

internal class OutboundMapperProfile : Profile
{
    public OutboundMapperProfile()
    {
        CreateMap<TtOutbound, Outbound>()
            .ForMember(s => s.Whs, db => db.MapFrom(i => i.WhsCode))
            .ForMember(s => s.Type, db => db.MapFrom(i => OutboundType.From(i.TransType)))
            .ForMember(s => s.Status, db => db.MapFrom(i => OutboundStatus.From(i.Status)))
            .ForMember(s => s.OrderNo, db => db.MapFrom(i => i.RefNo))
            .ForMember(s => s.Remarks, db => db.MapFrom(i => i.Remark));
    }
}
