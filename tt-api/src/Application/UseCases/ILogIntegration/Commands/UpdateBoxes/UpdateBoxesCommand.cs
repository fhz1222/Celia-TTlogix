using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.ILogIntegration.Commands.UpdateBoxes;

public class UpdateBoxesCommand : IRequest
{
    public BoxDto[] Boxes;
}

public class UpdateBoxesCommandHandler : IRequestHandler<UpdateBoxesCommand>
{
    private readonly IRepository repository;

    public UpdateBoxesCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public Task<Unit> Handle(UpdateBoxesCommand request, CancellationToken cancellationToken)
    {
        try
        {
            repository.BeginTransaction();
            repository.ILogBoxes.UpdateBoxes(request.Boxes);
            repository.CommitTransaction();
        }
        catch
        {
            repository.RollbackTransaction();
            throw;
        }

        return Task.FromResult(Unit.Value);
    }
}
