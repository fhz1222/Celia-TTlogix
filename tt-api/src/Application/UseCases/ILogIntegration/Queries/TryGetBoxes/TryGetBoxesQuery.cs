using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.ILogIntegration.Queries.TryGetBoxes;

public class TryGetBoxesQuery : IRequest<List<BoxDto>>
{
    public string[] Pids { get; set; }
}

public class TryGetBoxesQueryHandler : IRequestHandler<TryGetBoxesQuery, List<BoxDto>>
{
    private readonly IRepository repository;

    public TryGetBoxesQueryHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public Task<List<BoxDto>> Handle(TryGetBoxesQuery request, CancellationToken cancellationToken)
    {
        var boxes = repository.ILogBoxes.GetBoxes(request.Pids);
        return Task.FromResult(boxes);
    }
}