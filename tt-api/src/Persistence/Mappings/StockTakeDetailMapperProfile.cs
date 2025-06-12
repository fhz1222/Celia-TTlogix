using Application.UseCases.StockTake;
using AutoMapper;
using Domain.ValueObjects;
using Persistence.FilterHandlers.StockTakeTabs;

namespace Persistence.Mappings
{
    public class StockTakeDetailMapperProfile : Profile
    {
        public StockTakeDetailMapperProfile()
        {
            CreateMap<StockTakeItemWithStorageInfo, StockTakeItemDto>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (StorageStatus?)z.Status));
        }
    }
}
