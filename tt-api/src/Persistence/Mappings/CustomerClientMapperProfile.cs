using AutoMapper;
using Domain.Entities;
using Domain.ValueObjects;
using Persistence.Entities;

namespace Persistence.Mappings
{
    public class CustomerClientMapperProfile : Profile
    {
        public CustomerClientMapperProfile()
        {
            CreateMap<TtCustomerClient, CustomerClient>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (CustomerClientStatus)c.Status));

            CreateMap<CustomerClient, TtCustomerClient>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (byte)c.Status));
        }
    }
}
