using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.Registration.Commands.AddUom;

public class AddUomCommand : IRequest<Uom>
{
    public AddUomDto Dto { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class AddUomCommandHandler : IRequestHandler<AddUomCommand, Uom>
{
    private readonly IRepository repository;

    public AddUomCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Uom> Handle(AddUomCommand request, CancellationToken cancellationToken)
    {
        if(request.Dto.Name.Length == 0)
            throw new ApplicationError("Name cannot be empty.");

        repository.BeginTransaction();
        try
        {
            var code = repository.Utils.GetNextCode(Domain.Enums.CodePrefix.UOM);
            var Uom = new Uom
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

            repository.Metadata.AddNew(Common.EntityType.Uom, Uom);
            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();

            return Uom;
        }
        catch(Exception)
        {
            repository.RollbackTransaction();
            throw;
        }
    }
}
