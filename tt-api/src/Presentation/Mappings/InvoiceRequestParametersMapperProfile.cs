using Application.UseCases.InvoiceRequest.Queries.GetBatches;
using AutoMapper;
using Presentation.Common;

namespace Persistence.Mappings
{
    public class InvoiceRequestParametersMapperProfile : Profile
    {
        public InvoiceRequestParametersMapperProfile()
        {
            CreateMap<GetBatchesRequestParameters, GetBatchesQueryFilter>();
        }
    }
}
