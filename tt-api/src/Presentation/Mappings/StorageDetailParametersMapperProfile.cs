using Application.UseCases.Storage.Queries.GetStorageDetailItems;
using Application.UseCases.StorageDetails.Queries.GetStorageDetailItems;
using AutoMapper;
using Presentation.Common;

namespace Persistence.Mappings
{
    /// <summary>
    /// Automapper for adjustment parameters; maps StorageDetailItemParameters object to StorageDetailItemDtoFilter object
    /// </summary>
    public class StorageDetailParametersMapperProfile : Profile
    {
        /// <summary>
        /// default constructor - creates the map
        /// </summary>
        public StorageDetailParametersMapperProfile()
        {
            CreateMap<StorageDetailItemParameters, StorageDetailItemDtoFilter>()
                .ForMember(d => d.WhsCode, m => m.Ignore());

            CreateMap<StorageDetailItemWithPartInfoParameters, StorageDetailItemWithPartInfoDtoFilter>()
                .ForMember(d => d.WhsCode, m => m.Ignore());
        }
    }
}
