using Application.UseCases.StockTransferReversals;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Persistence.Entities;

namespace Persistence.Mappings;

internal class TtStfReversalMasterMapperProfile : Profile
{
    public TtStfReversalMasterMapperProfile()
    {
        CreateMap<TtStfReversalMaster, StockTransferReversalDto>()
            .ForMember(s => s.Status, db => db.MapFrom(i => (StockTransferStatus)i.Status));

        CreateMap<TtStfReversalMaster, StockTransferReversal>()
            .ForMember(s => s.Status, db => db.MapFrom(i => (StockTransferStatus)i.Status));

        CreateMap<StockTransferReversal, TtStfReversalMaster>()
            .ForMember(s => s.Status, db => db.MapFrom(i => (byte)i.Status));
    }
}
