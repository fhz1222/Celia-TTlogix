using AutoMapper;
using TT.Common;
using TT.Services.Models;

namespace TT.Services.Profiles
{
    public class AppSettingsProfile : Profile
    {
        public AppSettingsProfile()
        {
            CreateMap<AppSettings, AppSettingsDto>()
                .ForMember(dst => dst.OwnerCode, opt => opt.MapFrom(src => src.OwnerCode))
                .ForAllOtherMembers(dst => dst.Ignore());
        }
    }
}
