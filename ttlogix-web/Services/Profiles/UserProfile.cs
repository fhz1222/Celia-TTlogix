using AutoMapper;
using TT.Core.Entities;
using TT.Services.Models;

namespace TT.Services.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();

            CreateMap<UserAddDto, User>()
                .ForMember(dest => dest.Code, m => m.MapFrom(src => src.Code))
                .ForMember(dest => dest.FirstName, m => m.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, m => m.MapFrom(src => src.LastName))
                .ForMember(dest => dest.GroupCode, m => m.MapFrom(src => src.GroupCode))
                .ForMember(dest => dest.WHSCode, m => m.MapFrom(src => src.WHSCode))
                .ForMember(dest => dest.Password, m => m.MapFrom(src => src.Password))
                .ForMember(dest => dest.Status, m => m.MapFrom(src => src.Status))
                .ForAllOtherMembers(d => d.Ignore());

            CreateMap<UserUpdateDto, User>()
                .ForMember(dest => dest.FirstName, m => m.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, m => m.MapFrom(src => src.LastName))
                .ForMember(dest => dest.GroupCode, m => m.MapFrom(src => src.GroupCode))
                .ForMember(dest => dest.WHSCode, m => m.MapFrom(src => src.WHSCode))
                .ForMember(dest => dest.Password, m =>
                {
                    m.PreCondition(src => !string.IsNullOrEmpty(src.Password));
                    m.MapFrom(src => src.Password);
                })
                .ForAllOtherMembers(d => d.Ignore());

            CreateMap<User, CurrentUserDto>()
                .ForMember(dest => dest.Token, m => m.Ignore())
                .ForMember(dest => dest.Roles, m => m.Ignore());
        }
    }
}
