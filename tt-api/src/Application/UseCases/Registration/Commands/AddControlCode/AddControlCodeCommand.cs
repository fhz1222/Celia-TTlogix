using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.Registration.Commands.AddControlCode;

public class AddControlCodeCommand : IRequest<ControlCode>
{
    public AddControlCodeDto Dto { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class AddControlCodeCommandHandler : IRequestHandler<AddControlCodeCommand, ControlCode>
{
    private readonly IRepository repository;

    public AddControlCodeCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<ControlCode> Handle(AddControlCodeCommand request, CancellationToken cancellationToken)
    {
        if(request.Dto.Name.Length == 0)
            throw new ApplicationError("Name cannot be empty.");

        repository.BeginTransaction();
        try
        {
            var code = repository.Utils.GetNextCode(Domain.Enums.CodePrefix.ControlCode);
            var ControlCode = new ControlCode
            {
                Code = code,
                Name = request.Dto.Name,
                Type = request.Dto.Type,
                Status = Status.Active,
                CreatedBy = request.UserCode,
                CreatedDate = DateTime.Now,
                CancelledBy = "",
                CancelledDate = null,
            };

            repository.Metadata.AddNew(Common.EntityType.ControlCode, ControlCode);
            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();

            return ControlCode;
        }
        catch(Exception)
        {
            repository.RollbackTransaction();
            throw;
        }
    }
}
