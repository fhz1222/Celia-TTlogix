using Application.UseCases.StockTake.Queries.GetStockTakeList;
using AutoMapper;
using Presentation.Common;

namespace Persistence.Mappings
{
    /// <summary>
    /// Automapper for stock take parameters; maps GetStockTakeListParameters object to GetStockTakeListDtoFilter object
    /// </summary>
    public class GetStockTakeListParametersMapperProfile : Profile
    {
        /// <summary>
        /// default constructor - creates the map
        /// </summary>
        public GetStockTakeListParametersMapperProfile()
        {
            CreateMap<GetStockTakeListParameters, GetStockTakeListDtoFilter>();
        }
    }
}
