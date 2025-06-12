using AutoMapper;
using Domain.Entities;
using Persistence.Entities;

namespace Persistence.Mappings;

internal class InboundReversalDetailMapperProfile : Profile
{
    public InboundReversalDetailMapperProfile()
    {
        CreateMap<TtInboundReversalDetail, InboundReversalDetail>();

        CreateMap<InboundReversalDetail, TtInboundReversalDetail>();
    }
}
