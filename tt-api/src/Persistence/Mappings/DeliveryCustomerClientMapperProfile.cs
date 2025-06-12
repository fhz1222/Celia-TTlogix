using Application.UseCases.Delivery.Queries.GetDeliveryCustomerClientList;
using AutoMapper;
using Persistence.FilterHandlers.Delivery;

namespace Persistence.Mappings
{
    public class DeliveryCustomerClientMapperProfile : Profile
    {
        public DeliveryCustomerClientMapperProfile()
        {
            CreateMap<DeliveryCustomerClient, DeliveryCustomerClientDto>();
        }
    }
}
