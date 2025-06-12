using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.ILogIntegration.Commands.DeleteBoxes;

public class DeleteBoxesCommand : IRequest
{
    public string[] Boxes { get; set; }
}

public class DeleteBoxesCommandHandler : IRequestHandler<DeleteBoxesCommand>
{
    private readonly IILogBoxRepository repository;

    public DeleteBoxesCommandHandler(IILogBoxRepository repository)
    {
        this.repository = repository;
    }

    public Task<Unit> Handle(DeleteBoxesCommand request, CancellationToken cancellationToken)
    {
        repository.DeleteBoxes(request.Boxes);
        return Task.FromResult(Unit.Value);
    }
}