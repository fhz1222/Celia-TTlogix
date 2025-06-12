using Application.UseCases.Supplier;
using AutoMapper;
using Persistence.Entities;

namespace Persistence.Mappings
{
    public class SupplierMapperProfile : Profile
    {
        public SupplierMapperProfile()
        {
            CreateMap<SupplierMaster, SupplierDto>();
            CreateMap<SupplierDto, SupplierMaster>();
        }
    }
}
