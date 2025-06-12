using AutoMapper;
using TT.Core.Entities;
using TT.Services.Models;

namespace TT.Services.Profiles
{
    public class PickingListProfile : Profile
    {
        public PickingListProfile()
        {
            CreateMap<PickingList, PickingList>();

            CreateMap<PickingListEKanban, PickingListEKanban>();

            CreateMap<PickingAllocatedPID, PickingAllocatedPID>();

            CreateMap<PickingListAllocateDto, PickingList>()
                .ForMember(dst => dst.JobNo, m => m.MapFrom(src => src.JobNo))
                .ForMember(dst => dst.LineItem, m => m.MapFrom(src => src.LineItem))
                .ForMember(dst => dst.PID, m => m.MapFrom(src => src.PID))
                .ForAllOtherMembers(dst => dst.Ignore());
        }
    }
}