using AutoMapper;
using Persistence.Entities;

namespace Persistence.Mappings;

public class InvoiceRequestMapperProfile : Profile
{
    public InvoiceRequestMapperProfile()
    {
        CreateMap<InvoiceBatch, Domain.Entities.InvoiceBatch>();

        CreateMap<InvoiceRequest, Domain.Entities.InvoiceRequest>();

        CreateMap<InvoicePriceValidation, Domain.Entities.PriceValidation>();
    }
}