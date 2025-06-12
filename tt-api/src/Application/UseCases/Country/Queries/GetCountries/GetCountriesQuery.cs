using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Country.Queries.GetCountries;

public class GetCountriesQuery : IRequest<IEnumerable<CountryDto>>
{
}

public class GetCountriesQueryHandler : IRequestHandler<GetCountriesQuery, IEnumerable<CountryDto>>
{
    private readonly IRepository repository;

    public GetCountriesQueryHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public Task<IEnumerable<CountryDto>> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
    {
        var dto = repository.Countries.GetCountriesDto();
        return Task.FromResult(dto.AsEnumerable());
    }
}

