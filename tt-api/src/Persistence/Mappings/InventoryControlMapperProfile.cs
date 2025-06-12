using Application.UseCases.Customer;
using AutoMapper;
using Domain.Entities;
using Persistence.Entities;


namespace Persistence.Mappings
{
    public class InventoryControlMapperProfile : Profile
    {
        public InventoryControlMapperProfile()
        {
            CreateMap<TtInventoryControl, InventoryControlDto>();
            CreateMap<TtInventoryControl, InventoryControl>();
            CreateMap<InventoryControl, TtInventoryControl>();
        }
    }
}
