using Application.Extensions;
using Application.Interfaces.Repositories;
using Application.UseCases.Country;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class CountryRepository : ICountryRepository
{
    private readonly AppDbContext context;
    private readonly IMapper mapper;

    public CountryRepository(AppDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public bool CountryDoesNotExistsOrIsInactive(string countryCode)
    {
        var activeStatusValue = (byte)Status.Active;
        var notfound = context.Countries
            .AsNoTracking()
            .Where(x => x.Code == countryCode)
            .Where(x => x.Status == activeStatusValue)
            .None();
        return notfound;
    }

    public List<CountryDto> GetCountriesDto()
    {
        var activeStatusValue = (byte)Status.Active;
        var countries = context.Countries
            .AsNoTracking()
            .Where(x => x.Status == activeStatusValue)
            .ProjectTo<CountryDto>(mapper.ConfigurationProvider)
            .ToList();
        return countries;
    }
}
