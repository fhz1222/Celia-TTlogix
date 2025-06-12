using Application.UseCases.InboundReversalItems.Queries.GetInboundReversalItems;
using AutoMapper;
using Presentation.Common;

namespace Persistence.Mappings
{
    /// <summary>
    /// Automapper for inbound reversal items parameters; maps ReversibleInboundItemsParameters object to GetInboundReversalItemsDtoFilter object
    /// </summary>
    public class InboundReversalItemsParametersMapperProfile : Profile
    {
        /// <summary>
        /// default constructor - creates the map
        /// </summary>
        public InboundReversalItemsParametersMapperProfile()
        {
            CreateMap<InboundReversalItemsParameters, GetInboundReversalItemsDtoFilter>();
        }
    }
}
