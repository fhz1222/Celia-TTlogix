using Application.UseCases.Country;

namespace Application.Interfaces.Repositories
{
    public interface ICountryRepository
    {
        bool CountryDoesNotExistsOrIsInactive(string countryCode);
        List<CountryDto> GetCountriesDto();
    }
}