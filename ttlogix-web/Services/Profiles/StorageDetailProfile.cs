using AutoMapper;
using TT.Core.Entities;
using TT.Core.QueryResults;
using TT.Services.Models;

namespace TT.Services.Profiles
{
    public class StorageDetailProfile : Profile
    {
        public StorageDetailProfile()
        {
            CreateMap<StorageDetail, StorageDetail>();

            CreateMap<StorageDetailWithPartInfoQueryResult, StorageDetailWithPartInfoDto>()
                .ForMember(dest => dest.RefNo, opt => opt.MapFrom(src => src.RefNo))
                .ForMember(dest => dest.SPQ, opt => opt.MapFrom(src => src.SPQ))
                .ForMember(dest => dest.DecimalNum, opt => opt.MapFrom(src => src.DecimalNum))
                .ForMember(dest => dest.ExternalPID, opt => opt.MapFrom(src => src.ExternalPID))
                .ForMember(dest => dest.InboundDate, opt => opt.MapFrom(src => src.StorageDetail.InboundDate))
                .ForMember(dest => dest.LocationCode, opt => opt.MapFrom(src => src.StorageDetail.LocationCode))
                .ForMember(dest => dest.Ownership, opt => opt.MapFrom(src => src.StorageDetail.Ownership))
                .ForMember(dest => dest.PID, opt => opt.MapFrom(src => src.StorageDetail.PID))
                .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.StorageDetail.ProductCode))
                .ForMember(dest => dest.Qty, opt => opt.MapFrom(src => src.StorageDetail.Qty))
                .ForMember(dest => dest.SupplierID, opt => opt.MapFrom(src => src.StorageDetail.SupplierID))
                .ForMember(dest => dest.WHSCode, opt => opt.MapFrom(src => src.StorageDetail.WHSCode))
                .ForMember(dest => dest.GroupID, opt => opt.MapFrom(src => src.StorageDetail.GroupID));

            CreateMap<SFTStorageDetailWithPartInfoQueryResult, STFStorageDetailWithPartInfoDto>()
                .ForMember(dest => dest.PID, opt => opt.MapFrom(src => src.StorageDetail.PID))
                .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.StorageDetail.ProductCode))
                .ForMember(dest => dest.SupplierID, opt => opt.MapFrom(src => src.StorageDetail.SupplierID))
                .ForMember(dest => dest.Qty, opt => opt.MapFrom(src => src.StorageDetail.Qty))
                .ForMember(dest => dest.InboundDate, opt => opt.MapFrom(src => src.StorageDetail.InboundDate))
                .ForMember(dest => dest.LocationCode, opt => opt.MapFrom(src => src.StorageDetail.LocationCode))
                .ForMember(dest => dest.WHSCode, opt => opt.MapFrom(src => src.StorageDetail.WHSCode))
                .ForMember(dest => dest.DaysInStock, opt => opt.MapFrom(src => src.DaysInStock))
                .ForMember(dest => dest.Ownership, opt => opt.MapFrom(src => src.StorageDetail.Ownership));
        }
    }
}
