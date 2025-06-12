using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Metadata;
using Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Application.UseCases.Registration.Commands.DeleteLabelPrinter;

public class DeleteLabelPrinterCommand : IRequest<Unit>
{
    public string IP { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class DeleteLabelPrinterCommandHandler : IRequestHandler<DeleteLabelPrinterCommand, Unit>
{
    private readonly IRepository repository;

    public DeleteLabelPrinterCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Unit> Handle(DeleteLabelPrinterCommand request, CancellationToken cancellationToken)
    {
        if(!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new UnknownUserCodeException();

        repository.BeginTransaction();
        try
        {
            repository.Metadata.Remove(EntityType.LabelPrinter, new string[] { request.IP });

            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();
            return Unit.Value;
        }
        catch(Exception)
        {
            repository.RollbackTransaction();
            throw;
        }
    }
}
