using Application.UseCases.Customer;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Persistence.Entities;

namespace Persistence.Mappings
{
    public class CustomerMapperProfile : Profile
    {
        public CustomerMapperProfile()
        {
            CreateMap<TtCustomer, CustomerDto>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (CustomerStatus)c.Status));

            CreateMap<CustomerDto, TtCustomer>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (byte)c.Status));

            CreateMap<TtCustomer, Customer>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (CustomerStatus)c.Status));

            CreateMap<Customer, TtCustomer>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (byte)c.Status));
        }
    }
}
