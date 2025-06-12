using Application.UseCases.InboundReversals;
using AutoMapper;
using Domain.Entities;
using Domain.ValueObjects;
using Persistence.Entities;

namespace Persistence.Mappings;

internal class InboundReversalMapperProfile : Profile
{
    public InboundReversalMapperProfile()
    {
        CreateMap<TtInboundReversal, InboundReversalDto>()
            .ForMember(s => s.Status, db => db.MapFrom(i => (InboundReversalStatus)i.Status));

        CreateMap<TtInboundReversal, InboundReversal>()
            .ForMember(s => s.Status, db => db.MapFrom(i => (InboundReversalStatus)i.Status));

        CreateMap<InboundReversal, TtInboundReversal>()
            .ForMember(s => s.Status, db => db.MapFrom(i => (byte)i.Status));
    }
}
