using Application.UseCases.InboundReversalItems.Queries.GetReversibleInboundItems;
using AutoMapper;
using Presentation.Common;

namespace Persistence.Mappings
{
    /// <summary>
    /// Automapper for reversible inbound items parameters; maps ReversibleInboundItemsParameters object to GetReversibleInboundItemsDtoFilter object
    /// </summary>
    public class ReversibleInboundItemsParametersMapperProfile : Profile
    {
        /// <summary>
        /// default constructor - creates the map
        /// </summary>
        public ReversibleInboundItemsParametersMapperProfile()
        {
            CreateMap<ReversibleInboundItemsParameters, GetReversibleInboundItemsDtoFilter>();
        }
    }
}
