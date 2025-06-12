using Application.UseCases.InboundReversals.Queries.GetInboundReversals;
using AutoMapper;
using Presentation.Common;

namespace Persistence.Mappings
{
    /// <summary>
    /// Automapper for inbound reversal parameters; maps InboundReversalParameters object to GetInboundReversalsDtoFilter object
    /// </summary>
    public class InboundReversalParametersMapperProfile : Profile
    {
        /// <summary>
        /// default constructor - creates the map
        /// </summary>
        public InboundReversalParametersMapperProfile()
        {
            CreateMap<InboundReversalParameters, GetInboundReversalsDtoFilter>();
        }
    }
}
