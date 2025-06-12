using AutoMapper;
using TT.Core.Entities;
using TT.Services.Models;

namespace TT.Services.Profiles
{
    public class LocationProfile : Profile
    {
        public LocationProfile()
        {
            CreateMap<Location, LocationDto>();
        }
    }
}
