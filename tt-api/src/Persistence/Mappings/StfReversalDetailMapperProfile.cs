using AutoMapper;
using Domain.Entities;
using Persistence.Entities;

namespace Persistence.Mappings;

internal class StfReversalDetailMapperProfile : Profile
{
    public StfReversalDetailMapperProfile()
    {
        CreateMap<TtStfReversalDetail, StockTransferReversalDetail>();

        CreateMap<StockTransferReversalDetail, TtStfReversalDetail>();
    }
}
