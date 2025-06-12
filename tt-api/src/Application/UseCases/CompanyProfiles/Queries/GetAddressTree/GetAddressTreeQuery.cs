using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.CompanyProfiles.Queries.GetAddressTree;

public class GetAddressTreeQuery : IRequest<AddressTreeDto>
{
}

public class GetAddressTreeQueryHandler : IRequestHandler<GetAddressTreeQuery, AddressTreeDto>
{
    private readonly IRepository repository;

    public GetAddressTreeQueryHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public Task<AddressTreeDto> Handle(GetAddressTreeQuery request, CancellationToken cancellationToken)
    {
        var dto = repository.CompanyProfiles.GetAddressTreeDto();
        return Task.FromResult(dto);
    }
}

