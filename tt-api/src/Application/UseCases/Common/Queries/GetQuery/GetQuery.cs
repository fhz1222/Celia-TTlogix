using Application.Interfaces.Repositories;
using MediatR;
using Application.Exceptions;

namespace Application.UseCases.Common.Queries.GetListQuery;

public class GetQuery<T> : IRequest<T>
    where T: class
{
    public string[] Key { get; set; } = null!;
    public EntityType EntityType { get; set; }
}

public class GetQueryHandler<T> : IRequestHandler<GetQuery<T>,T>
    where T: class
{
    private readonly IRepository repository;

    public GetQueryHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public Task<T> Handle(GetQuery<T> request, CancellationToken cancellationToken)
    {
        var item = repository.Metadata.Get<T>(request.EntityType, request.Key)
            ?? throw new ApplicationError($"Cannot find {request.EntityType} for {string.Join(' ', request.Key)}");

        return Task.FromResult(item);
    }
}
