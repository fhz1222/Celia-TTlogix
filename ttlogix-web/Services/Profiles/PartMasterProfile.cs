using AutoMapper;
using TT.Core.Entities;
using TT.Core.Enums;
using TT.Services.Models;

namespace TT.Services.Profiles
{
    public class PartMasterProfile : Profile
    {
        public PartMasterProfile()
        {
            CreateMap<PartMaster, PartMasterDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (ValueStatus)src.Status))
                .ForMember(dest => dest.EnableSerialNo, opt => opt.MapFrom(src => src.EnableSerialNo == 1))
                .ForMember(dest => dest.IsDefected, opt => opt.MapFrom(src => src.IsDefected == 1))
                .ForMember(dest => dest.IsStandardPackaging, opt => opt.MapFrom(src => src.IsStandardPackaging == 1))
                .ForMember(dest => dest.IsCPart, opt => opt.MapFrom(src => src.IsCPart == 1))
                .ForMember(dest => dest.IsPalletItem, opt => opt.MapFrom(src => src.IsPalletItem == 1))
                .ForMember(dest => dest.SupplierName, opt => opt.Ignore())
                .ForMember(dest => dest.LengthInternal, opt => opt.MapFrom(src => src.LengthTT))
                .ForMember(dest => dest.WidthInternal, opt => opt.MapFrom(src => src.WidthTT))
                .ForMember(dest => dest.HeightInternal, opt => opt.MapFrom(src => src.HeightTT))
                .ForMember(dest => dest.NetWeightInternal, opt => opt.MapFrom(src => src.NetWeightTT))
                .ForMember(dest => dest.GrossWeightInternal, opt => opt.MapFrom(src => src.GrossWeightTT));

            CreateMap<PartMasterDto, PartMaster>()
                .ForMember(dest => dest.CustomerCode, opt => opt.MapFrom(src => src.CustomerCode))
                .ForMember(dest => dest.SupplierID, opt => opt.MapFrom(src => src.SupplierID))
                .ForMember(dest => dest.ProductCode1, opt => opt.MapFrom(src => src.ProductCode1))
                .ForMember(dest => dest.CPartSPQ, opt => opt.MapFrom(src => src.CPartSPQ))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.EnableSerialNo, opt => opt.MapFrom(src => src.EnableSerialNo))
                .ForMember(dest => dest.GrossWeight, opt => opt.MapFrom(src => src.GrossWeight))
                .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.Height))
                .ForMember(dest => dest.IsCPart, opt => opt.MapFrom(src => src.IsCPart))
                .ForMember(dest => dest.IsDefected, opt => opt.MapFrom(src => src.IsDefected))
                .ForMember(dest => dest.IsPalletItem, opt => opt.MapFrom(src => src.IsPalletItem))
                .ForMember(dest => dest.IsStandardPackaging, opt => opt.MapFrom(src => src.IsStandardPackaging))
                .ForMember(dest => dest.Length, opt => opt.MapFrom(src => src.Length))
                .ForMember(dest => dest.NetWeight, opt => opt.MapFrom(src => src.NetWeight))
                .ForMember(dest => dest.OrderLot, opt => opt.MapFrom(src => src.OrderLot))
                .ForMember(dest => dest.OriginCountry, opt => opt.MapFrom(src => src.OriginCountry ?? string.Empty))
                .ForMember(dest => dest.PackageType, opt => opt.MapFrom(src => src.PackageType))
                .ForMember(dest => dest.ProductCode2, opt => opt.MapFrom(src => src.ProductCode2 ?? string.Empty))
                .ForMember(dest => dest.ProductCode3, opt => opt.MapFrom(src => src.ProductCode3 ?? string.Empty))
                .ForMember(dest => dest.ProductCode4, opt => opt.MapFrom(src => src.ProductCode4 ?? string.Empty))
                .ForMember(dest => dest.SPQ, opt => opt.MapFrom(src => src.SPQ))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (byte)src.Status))
                .ForMember(dest => dest.UOM, opt => opt.MapFrom(src => src.UOM))
                .ForMember(dest => dest.Width, opt => opt.MapFrom(src => src.Width))
                .ForMember(dest => dest.MasterSlave, opt => opt.MapFrom(src => src.MasterSlave))
                .ForMember(dest => dest.BoxItem, opt => opt.MapFrom(src => src.BoxItem))
                .ForMember(dest => dest.FloorStackability, opt => opt.MapFrom(src => src.FloorStackability))
                .ForMember(dest => dest.TruckStackability, opt => opt.MapFrom(src => src.TruckStackability))
                .ForMember(dest => dest.BoxesInPallet, opt => opt.MapFrom(src => src.BoxesInPallet))
                .ForMember(dest => dest.DoNotSyncEDI, opt => opt.MapFrom(src => src.DoNotSyncEDI))
                .ForMember(dest => dest.iLogReadinessStatus, opt => opt.MapFrom(src => src.iLogReadinessStatus))
                .ForMember(dest => dest.IsMixed, opt => opt.MapFrom(src => src.IsMixed))
                .ForMember(dest => dest.UnloadingPointId, opt => opt.MapFrom(src => src.UnloadingPointId))
                .ForMember(dest => dest.LengthTT, opt => opt.MapFrom(src => src.LengthInternal))
                .ForMember(dest => dest.WidthTT, opt => opt.MapFrom(src => src.WidthInternal))
                .ForMember(dest => dest.HeightTT, opt => opt.MapFrom(src => src.HeightInternal))
                .ForMember(dest => dest.NetWeightTT, opt => opt.MapFrom(src => src.NetWeightInternal))
                .ForMember(dest => dest.GrossWeightTT, opt => opt.MapFrom(src => src.GrossWeightInternal))
                .ForMember(dest => dest.PalletTypeId, opt => opt.MapFrom(src => src.PalletTypeId))
                .ForMember(dest => dest.ELLISPalletTypeId, opt => opt.MapFrom(src => src.ELLISPalletTypeId))
                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}
