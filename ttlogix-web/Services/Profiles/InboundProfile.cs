using AutoMapper;
using TT.Core.Entities;
using TT.Core.Enums;
using TT.Services.Models;

namespace TT.Services.Profiles
{
    public class InboundProfile : Profile
    {
        public InboundProfile()
        {
            CreateMap<InboundManualDto, Inbound>()
                .ForMember(dest => dest.CustomerCode, opt => opt.MapFrom(src => src.CustomerCode))
                .ForMember(dest => dest.SupplierID, opt => opt.MapFrom(src => src.SupplierID))
                .ForMember(dest => dest.WHSCode, opt => opt.MapFrom(src => src.WHSCode))
                .ForMember(dest => dest.TransType, opt => opt.MapFrom(src => src.TransType))
                .ForMember(dest => dest.RefNo, opt => opt.MapFrom(src => src.RefNo ?? string.Empty))
                .ForMember(dest => dest.IRNo, opt => opt.MapFrom(src => src.IRNo ?? string.Empty))
                .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.Remark ?? string.Empty))
                .ForMember(dest => dest.ETA, opt => opt.MapFrom(src => src.ETA))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => InboundStatus.NewJob))
                .ForAllOtherMembers(dest => dest.Ignore());

            //CreateMap<Inbound, InboundDto>()
            //  .ForMember(dest => dest.CustomerName, opt => opt.Ignore())
            //  .ForMember(dest => dest.SupplierName, opt => opt.Ignore())
            //  .ForMember(dest => dest.CreatedByName, opt => opt.Ignore())
            //  .ForMember(dest => dest.ContainerNo, opt => opt.Ignore());

            CreateMap<InboundDto, Inbound>()
                .ForMember(dest => dest.RefNo, opt =>
                {
                    opt.Condition((src, d) => d.Status != InboundStatus.Completed);
                    opt.MapFrom(src => src.RefNo ?? string.Empty);
                })
                .ForMember(dest => dest.Remark, opt =>
                {
                    opt.Condition((src, d) => d.Status != InboundStatus.Completed);
                    opt.MapFrom(src => src.Remark ?? string.Empty);
                })
                .ForMember(dest => dest.ETA, opt =>
                {
                    opt.Condition((src, d) => d.Status != InboundStatus.Completed);
                    opt.MapFrom(src => src.ETA);
                })
                .ForMember(dest => dest.CustomsDeclarationDate, opt => opt.MapFrom(src => src.CustomsDeclarationDate))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency ?? string.Empty))
                .ForMember(dest => dest.IM4No, opt => opt.MapFrom(src => src.IM4No ?? string.Empty))
                .ForAllOtherMembers(dest => dest.Ignore());

            CreateMap<InboundDetailEntryModifyDto, InboundDetail>()
                .ForMember(dest => dest.NoOfPackage, opt => opt.MapFrom((src, dst) => src.GetNoOfPackageAndLabel(dst.Qty)))
                .ForMember(dest => dest.NoOfLabel, opt => opt.MapFrom((src, dst) => src.GetNoOfPackageAndLabel(dst.Qty)))
                .ForAllOtherMembers(dest => dest.Ignore());

            CreateMap<InboundDetailEntryAddDto, InboundDetail>()
                .ForMember(dest => dest.JobNo, opt => opt.MapFrom(src => src.JobNo))
                .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.ProductCode))
                .ForMember(dest => dest.ImportedQty, opt => opt.MapFrom(src => src.ImportedQty))
                .ForMember(dest => dest.Qty, opt => opt.MapFrom(src => src.Qty))
                .ForMember(dest => dest.NoOfPackage, opt => opt.MapFrom(src => src.GetNoOfPackageAndLabel(src.Qty)))
                .ForMember(dest => dest.NoOfLabel, opt => opt.MapFrom(src => src.GetNoOfPackageAndLabel(src.Qty)))
                .ForMember(dest => dest.PackageType, opt => opt.MapFrom(src => src.PackageType))
                .ForMember(dest => dest.Length, opt => opt.MapFrom(src => src.Length))
                .ForMember(dest => dest.Width, opt => opt.MapFrom(src => src.Width))
                .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.Height))
                .ForMember(dest => dest.NetWeight, opt => opt.MapFrom(src => src.NetWeight))
                .ForMember(dest => dest.GrossWeight, opt => opt.MapFrom(src => src.GrossWeight))
                .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.Remark ?? string.Empty))
                .ForMember(dest => dest.ControlCode1, opt => opt.MapFrom(src => src.ControlCode1 ?? string.Empty))
                .ForMember(dest => dest.ControlCode2, opt => opt.MapFrom(src => src.ControlCode2 ?? string.Empty))
                .ForMember(dest => dest.ControlCode3, opt => opt.MapFrom(src => src.ControlCode3 ?? string.Empty))
                .ForMember(dest => dest.ControlCode4, opt => opt.MapFrom(src => src.ControlCode4 ?? string.Empty))
                .ForMember(dest => dest.ControlCode5, opt => opt.MapFrom(src => src.ControlCode5 ?? string.Empty))
                .ForMember(dest => dest.ControlCode6, opt => opt.MapFrom(src => src.ControlCode6 ?? string.Empty))
                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}

