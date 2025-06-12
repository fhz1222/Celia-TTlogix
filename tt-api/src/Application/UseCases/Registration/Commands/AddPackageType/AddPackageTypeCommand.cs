using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.Registration.Commands.AddPackageType;

public class AddPackageTypeCommand : IRequest<PackageType>
{
    public AddPackageTypeDto Dto { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class AddPackageTypeCommandHandler : IRequestHandler<AddPackageTypeCommand, PackageType>
{
    private readonly IRepository repository;

    public AddPackageTypeCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<PackageType> Handle(AddPackageTypeCommand request, CancellationToken cancellationToken)
    {
        if(request.Dto.Name.Length == 0)
            throw new ApplicationError("Name cannot be empty.");

        repository.BeginTransaction();
        try
        {
            var code = repository.Utils.GetNextCode(Domain.Enums.CodePrefix.PackageType);
            var packageType = new PackageType
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

            repository.Metadata.AddNew(Common.EntityType.PackageType, packageType);
            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();

            return packageType;
        }
        catch(Exception)
        {
            repository.RollbackTransaction();
            throw;
        }
    }
}
