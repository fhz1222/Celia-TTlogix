using AutoMapper;
using TT.Core.Entities;

namespace TT.Services.Profiles
{
    public class EOrderProfile : Profile
    {
        public EOrderProfile()
        {
            CreateMap<EOrder, EOrder>();
        }
    }
}
