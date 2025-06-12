using Application.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Domain.Metadata;
using FluentValidation;
using MediatR;

namespace Application.UseCases.Common.Commands.AddCommand;

public class AddCommand<DTO, T> : IRequest<T>
    where T : class
    where DTO : class
{
    public string[] Key { get; set; } = null!;
    public DTO Dto { get; set; } = null!;
    public EntityType EntityType { get; set; }
    public string UserCode { get; set; } = null!;
}

public class AddCommandHandler<DTO, T> : IRequestHandler<AddCommand<DTO, T>, T>
    where T : class
    where DTO : class
{
    private readonly IRepository repository;
    private readonly IDateTime dateTimeService;
    private readonly IValidator<DTO>? validator;

    public AddCommandHandler(IRepository repository, IDateTime dateTimeService, IValidator<DTO>? validator = null)
    {
        this.repository = repository;
        this.dateTimeService = dateTimeService;
        this.validator = validator;
    }

    public async Task<T> Handle(AddCommand<DTO, T> request, CancellationToken cancellationToken)
    {
        if(!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new UnknownUserCodeException();
                
        if (validator is not null && (validator.Validate(request.Dto) is var validationResult) && !validationResult.IsValid)
        {
            var errorMessage = string.Join(' ', validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ApplicationError(errorMessage);
        }

        repository.BeginTransaction();
        try
        {
            var metadata = Metadata.Create(request.UserCode, dateTimeService.Now);
            repository.Metadata.AddNewWithMetadata(request.EntityType, request.Dto, metadata);

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
