using Application.Exceptions;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Decants.Queries.GetDecantDetail;

public class GetDecantDetailsQuery : IRequest<Decant>
{
    public string JobNo;
}

public class GetDecantDetailQueryHandler : IRequestHandler<GetDecantDetailsQuery, Decant>
{
    private readonly IDecantRepository repository;

    public GetDecantDetailQueryHandler(IDecantRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Decant> Handle(GetDecantDetailsQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetDecant(request.JobNo);
        if (result == null)
            throw new UnknownJobNoException();
        return result;
    }
}

