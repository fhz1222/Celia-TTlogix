using Application.UseCases.Country;
using AutoMapper;
using Persistence.Entities;

namespace Persistence.Mappings
{
    public class CountryMapperProfile : Profile
    {
        public CountryMapperProfile()
        {
            CreateMap<TtCountry, CountryDto>();
        }
    }
}
