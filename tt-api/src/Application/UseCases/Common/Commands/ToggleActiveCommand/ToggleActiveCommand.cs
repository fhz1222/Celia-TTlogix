using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Domain.Metadata;
using MediatR;

namespace Application.UseCases.Common.Commands.ToggleActiveCommand;

public class ToggleActiveCommand<T> : IRequest<T>
    where T : class
{
    public string[] Key { get; set; } = null!;
    public EntityType EntityType { get; set; }
    public string UserCode { get; set; } = null!;
}

public class ToggleActiveCommandHandler<T> : IRequestHandler<ToggleActiveCommand<T>, T>
    where T : class
{
    private readonly IRepository repository;
    private readonly IDateTime dateTimeService;

    public ToggleActiveCommandHandler(IRepository repository, IDateTime dateTimeService)
    {
        this.repository = repository;
        this.dateTimeService = dateTimeService;
    }

    public async Task<T> Handle(ToggleActiveCommand<T> request, CancellationToken cancellationToken)
    {
        if(!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new UnknownUserCodeException();

        var obj = repository.Metadata.Get<StatusCancel>(request.EntityType, request.Key)
            ?? throw new ApplicationError($"Cannot find object for {string.Join(' ', request.Key)}.");

        repository.BeginTransaction();
        try
        {
            obj.ToggleStatusWithCancel(request.UserCode, dateTimeService.Now);
            
            repository.Metadata.Update(request.EntityType, obj, request.Key);
            
            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();
            return repository.Metadata.Get<T>(request.EntityType, request.Key)
                ?? throw new ApplicationError($"Cannot find object for {string.Join(' ', request.Key)}.");
        }
        catch(Exception)
        {
            repository.RollbackTransaction();
            throw;
        }
    }
}
