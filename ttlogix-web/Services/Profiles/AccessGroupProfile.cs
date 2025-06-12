using AutoMapper;
using TT.Core.Entities;
using TT.Services.Models;

namespace TT.Services.Profiles
{
    public class AccessGroupProfile : Profile
    {
        public AccessGroupProfile()
        {
            CreateMap<AccessGroupAddDto, AccessGroup>()
                .ForMember(dest => dest.Code, m => m.MapFrom(src => src.Code))
                .ForMember(dest => dest.Description, m => m.MapFrom(src => src.Description))
                .ForMember(dest => dest.Status, m => m.MapFrom(src => src.Status))
                .ForAllOtherMembers(d => d.Ignore());

            CreateMap<AccessGroupDto, AccessGroup>()
                .ForMember(dest => dest.Description, m => m.MapFrom(src => src.Description))
                .ForAllOtherMembers(d => d.Ignore());

            CreateMap<AccessGroup, AccessGroupDto>();
        }
    }
}
