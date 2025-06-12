using AutoMapper;
using TT.Core.Entities;
using TT.Core.Enums;
using TT.Services.Models;

namespace TT.Services.Profiles
{
    public class StockTransferProfile : Profile
    {
        public StockTransferProfile()
        {
            CreateMap<StockTransfer, StockTransferDto>()
                .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.TransferTypeString, opt => opt.MapFrom(src => src.TransferType.ToString()))
                .ForMember(dest => dest.Currency, opt => opt.Ignore())
                .ForMember(dest => dest.IsMixedCurrency, opt => opt.Ignore())
                .ForMember(dest => dest.OutboundTotalValue, opt => opt.Ignore());

            CreateMap<StockTransferDto, StockTransfer>()
                .ForMember(dest => dest.RefNo, opt =>
                {
                    opt.Condition((src, d) => d.Status != StockTransferStatus.Cancelled && d.Status != StockTransferStatus.Completed && d.TransferType != StockTransferType.EStockTransfer);
                    opt.MapFrom(src => src.RefNo ?? string.Empty);
                })
                .ForMember(dest => dest.Remark, opt =>
                {
                    opt.Condition((src, d) => d.Status != StockTransferStatus.Cancelled && d.Status != StockTransferStatus.Completed);
                    opt.MapFrom(src => src.Remark ?? string.Empty);
                })
                .ForMember(dest => dest.CommInvNo, opt => opt.MapFrom(src => src.CommInvNo ?? ""))
                .ForMember(dest => dest.CommInvDate, opt => opt.MapFrom(src => src.CommInvDate))
                .ForMember(dest => dest.TransferType, opt =>
                {
                    opt.Condition((src, d) => d.Status != StockTransferStatus.Cancelled && d.Status != StockTransferStatus.Completed);
                    opt.MapFrom(src => src.TransferType);
                })
                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}

