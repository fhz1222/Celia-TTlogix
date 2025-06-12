using Application.UseCases.Country;
using Application.UseCases.Country.Queries.GetCountries;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;
/// <summary>
/// CountryController provides method to get the country list
/// </summary>
public partial class CountryController : ApiControllerBase
{
    /// <summary>
    /// constructor
    /// </summary>
    public CountryController()
    {
    }

    /// <summary>
    /// Gets active Countries
    /// </summary>
    [HttpGet]
    [Produces("application/json")]
    public async Task<IEnumerable<CountryDto>> GetCountries()
    {
        return await Mediator.Send(new GetCountriesQuery());
    }
}


