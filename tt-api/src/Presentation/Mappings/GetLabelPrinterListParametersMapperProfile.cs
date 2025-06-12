using Application.UseCases.Registration.Queries.GetLabelPrinterList;
using AutoMapper;
using Presentation.Common;

namespace Presentation.Mappings
{
    /// <summary>
    /// Automapper for registration list parameters
    /// </summary>
    public class GetLabelPrinterListParametersMapperProfile : Profile
    {
        /// <summary>
        /// default constructor - creates the map
        /// </summary>
        public GetLabelPrinterListParametersMapperProfile()
        {
            CreateMap<GetLabelPrinterListParameters, GetLabelPrinterListDtoFilter>();
        }
    }
}
