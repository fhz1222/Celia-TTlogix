using Application.UseCases.StockTransferReversals.Queries.GetStockTransferReversals;
using AutoMapper;
using Presentation.Common;

namespace Presentation.Mappings
{
    /// <summary>
    /// Automapper for stock transfer reversal parameters; maps StockTransferParameters object to GetStockTransferReversalsDtoFilter object
    /// </summary>
    public class StockTransferReversalParametersMapperProfile : Profile
    {
        /// <summary>
        /// default constructor - creates the map
        /// </summary>
        public StockTransferReversalParametersMapperProfile()
        {
            CreateMap<StockTransferParameters, GetStockTransferReversalsDtoFilter>();
        }
    }
}
