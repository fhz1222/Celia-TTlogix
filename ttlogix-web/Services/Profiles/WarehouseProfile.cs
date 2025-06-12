using AutoMapper;
using TT.Core.Entities;
using TT.Services.Models;

namespace TT.Services.Profiles
{
    public class WarehouseProfile : Profile
    {
        public WarehouseProfile()
        {
            CreateMap<Warehouse, WarehouseDto>();
        }
    }
}
