using AutoMapper;
using Domain.Entities;
using Persistence.Entities;

namespace Persistence.Mappings
{
    public class StockTakeRelocationLogMapperProfile : Profile
    {
        public StockTakeRelocationLogMapperProfile()
        {
            CreateMap<StockTakeRelocationLog, TtStockTakeRelocationLog>()
                .ForPath(x => x.TransType, db => db.MapFrom(x => (byte)x.TransType));
        }
    }
}
