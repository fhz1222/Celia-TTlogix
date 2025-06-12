using AutoMapper;
using System.Reflection;
using TT.Core.Entities;
using TT.Services.Models;

namespace TT.Services.Profiles
{
    public class LoadingProfile : Profile
    {
        public LoadingProfile()
        {
            CreateMap<Loading, LoadingDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.CustomerName, opt => opt.Ignore())
                .ForMember(dest => dest.Currency, opt => opt.Ignore())
                .ForMember(dest => dest.CalculatedNoOfPallet, opt => opt.Ignore())
                .ForMember(dest => dest.MixedCurrencies, opt => opt.Ignore())
                .ForMember(dest => dest.MaxOutboundStatus, opt => opt.Ignore())
                .ForMember(dest => dest.MinOutboundStatus, opt => opt.Ignore());

            CreateMap<LoadingAddDto, Loading>()
                .ForMember(dest => dest.JobNo, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.RevisedBy, opt => opt.Ignore())
                .ForMember(dest => dest.RevisedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CancelledBy, opt => opt.Ignore())
                .ForMember(dest => dest.CancelledDate, opt => opt.Ignore())
                .ForMember(dest => dest.ConfirmedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ConfirmedDate, opt => opt.Ignore())
                .ForMember(dest => dest.NoOfPallet, opt => opt.Ignore())
                .ForMember(dest => dest.TruckArrivalDate, opt => opt.Ignore())
                .ForMember(dest => dest.TruckDepartureDate, opt => opt.Ignore())
                .ForMember(dest => dest.TruckLicencePlate, opt => opt.Ignore())
                .ForMember(dest => dest.TrailerNo, opt => opt.Ignore())
                .ForMember(dest => dest.DockNo, opt => opt.Ignore())
                .ForMember(dest => dest.TruckSeqNo, opt => opt.Ignore())
                .ForMember(dest => dest.ETA, opt => opt.Ignore())
                .ForMember(dest => dest.AllowedForDispatch, opt => opt.Ignore())
                .ForMember(dest => dest.AllowedForDispatchModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.AllowedForDispatchModifiedBy, opt => opt.Ignore())
                .ForAllOtherMembers(o =>
                {
                    if (((PropertyInfo)o.DestinationMember).PropertyType == typeof(string))
                    {
                        o.NullSubstitute("");
                    }
                    if (((PropertyInfo)o.DestinationMember).PropertyType == typeof(int?))
                    {
                        o.NullSubstitute(0);
                    }
                });

            CreateMap<LoadingDto, Loading>()
                .ForMember(dest => dest.RefNo, opt => opt.MapFrom(src => src.RefNo))
                .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.Remark))
                .ForMember(dest => dest.NoOfPallet, opt => opt.MapFrom(src => src.NoOfPallet))
                .ForMember(dest => dest.ETD, opt => opt.MapFrom(src => src.ETD))
                .ForMember(dest => dest.ETA, opt => opt.MapFrom(src => src.ETA))
                .ForMember(dest => dest.TruckLicencePlate, opt => opt.MapFrom(src => src.TruckLicencePlate))
                .ForMember(dest => dest.TrailerNo, opt => opt.MapFrom(src => src.TrailerNo))
                .ForMember(dest => dest.DockNo, opt => opt.MapFrom(src => src.DockNo))
                .ForMember(dest => dest.TruckSeqNo, opt => opt.MapFrom(src => src.TruckSeqNo))
                .ForMember(dest => dest.AllowedForDispatch, opt => opt.MapFrom(src => src.AllowedForDispatch))
                .ForAllOtherMembers(dest => dest.Ignore());

            CreateMap<LoadingEntryDto, LoadingDetail>()
                .ForMember(dest => dest.OrderNo, opt => opt.MapFrom(src => src.OrderNo))
                .ForMember(dest => dest.SupplierID, opt => opt.MapFrom(src => src.SupplierID))
                .ForMember(dest => dest.ETD, opt => opt.MapFrom(src => src.ETD))
                .ForMember(dest => dest.OutJobNo, opt => opt.MapFrom(src => src.OutboundJobNo))
                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}
