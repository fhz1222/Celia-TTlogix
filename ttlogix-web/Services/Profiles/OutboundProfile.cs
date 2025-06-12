using AutoMapper;
using System;
using System.Data;
using System.Linq;
using System.Reflection;
using TT.Core.Entities;
using TT.Core.Enums;
using TT.Services.Models;

namespace TT.Services.Profiles
{
    public class OutboundProfile : Profile
    {
        public OutboundProfile()
        {
            CreateMap<Outbound, OutboundDto>()
                .ForMember(dest => dest.TransType, opt => opt.MapFrom(src => src.TransType))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.ReportsPrinted, opt => opt.Ignore())
                .ForMember(dest => dest.CalculatedNoOfPallet, opt => opt.Ignore())
                .ForMember(dest => dest.AllowAutoallocation, opt => opt.Ignore())
                .ForMember(dest => dest.ShowOrderSummary, opt => opt.Ignore());

            CreateMap<OutboundDetailAddDto, OutboundDetail>(MemberList.Source);

            CreateMap<OutboundDto, Outbound>()
                .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.Remark))
                .ForMember(dest => dest.RefNo, opt =>
                {
                    var disabledStatuses = new OutboundStatus[] { OutboundStatus.Completed, OutboundStatus.Cancelled, OutboundStatus.InTransit, OutboundStatus.Discrepancy };
                    opt.Condition((src, d) => d.TransType == OutboundType.Return && !disabledStatuses.Contains(d.Status));

                    opt.MapFrom(src => src.RefNo);
                })
                .ForMember(dest => dest.ETD, opt => opt.MapFrom(src => src.ETD))
                .ForMember(dest => dest.CommInvNo, opt => opt.MapFrom(src => src.CommInvNo))
                .ForMember(dest => dest.NoOfPallet, opt => opt.MapFrom(src => src.NoOfPallet))
                .ForMember(dest => dest.DeliveryTo, opt => opt.MapFrom(src => src.DeliveryTo))
                .ForMember(dest => dest.XDock, opt => opt.MapFrom(src => src.XDock))
                .ForMember(dest => dest.TransportNo, opt => opt.MapFrom(src => src.TransportNo))
                .ForAllOtherMembers(dest => dest.Ignore());

            CreateMap<OutboundManualDto, Outbound>()
                .ForMember(dest => dest.NoOfPallet, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.JobNo, opt => opt.Ignore())
                .ForMember(dest => dest.Charged, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.RevisedBy, opt => opt.Ignore())
                .ForMember(dest => dest.RevisedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CancelledBy, opt => opt.Ignore())
                .ForMember(dest => dest.CancelledDate, opt => opt.Ignore())
                .ForMember(dest => dest.DispatchedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DispatchedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CommInvNo, opt => opt.Ignore())
                .ForMember(dest => dest.DeliveryTo, opt => opt.Ignore())
                .ForMember(dest => dest.XDock, opt => opt.Ignore())
                .ForMember(dest => dest.TransportNo, opt => opt.Ignore())
                .ForAllOtherMembers(o =>
                {
                    if (((PropertyInfo)o.DestinationMember).PropertyType == typeof(string))
                    {
                        o.NullSubstitute("");
                    }
                });

            CreateMap<OutboundDetail, OutboundDetail>();

            CreateMap<IDataRecord, EDTDataDto>(MemberList.Source)
                .ForSourceMember(src => src.FieldCount, d => d.DoNotValidate());
        }
    }
}

