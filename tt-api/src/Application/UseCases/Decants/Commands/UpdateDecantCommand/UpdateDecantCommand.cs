using Application.Exceptions;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Decants.Commands.UpdateDecantCommand;

public class UpdateDecantCommand : IRequest<Decant>
{
    public string JobNo { get; set; } = null!;
    public string? ReferenceNo { get; set; } = null!;
    public string? Remark { get; set; } = null!;
}

public class UpdateDecantCommandHandler : IRequestHandler<UpdateDecantCommand, Decant>
{
    private readonly IRepository repository;

    public UpdateDecantCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Decant> Handle(UpdateDecantCommand request, CancellationToken cancellationToken)
    {
        repository.BeginTransaction();
        var decant = await repository.Decant.GetDecant(request.JobNo);
        try
        {
            if (decant == null)
                throw new UnknownJobNoException();

            if (!decant.CanEdit)
                throw new IllegalAdjustmentChangeException($"Decant with status 'Cancelled' or 'Completed' cannot be updated");

            decant.ReferenceNo = request.ReferenceNo;
            decant.Remark = request.Remark;
            await repository.Decant.UpdateDecant(decant);
            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();
            return decant;
        }
        catch (Exception)
        {
            repository.RollbackTransaction();
            throw;
        }
    }
}
