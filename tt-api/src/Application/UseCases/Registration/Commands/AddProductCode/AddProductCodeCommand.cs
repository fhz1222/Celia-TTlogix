using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.Registration.Commands.AddProductCode;

public class AddProductCodeCommand : IRequest<ProductCode>
{
    public AddProductCodeDto Dto { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class AddProductCodeCommandHandler : IRequestHandler<AddProductCodeCommand, ProductCode>
{
    private readonly IRepository repository;

    public AddProductCodeCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<ProductCode> Handle(AddProductCodeCommand request, CancellationToken cancellationToken)
    {
        if(request.Dto.Name.Length == 0)
            throw new ApplicationError("Name cannot be empty.");

        repository.BeginTransaction();
        try
        {
            var code = repository.Utils.GetNextCode(Domain.Enums.CodePrefix.ProductCode);
            var ProductCode = new ProductCode
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

            repository.Metadata.AddNew(Common.EntityType.ProductCode, ProductCode);
            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();

            return ProductCode;
        }
        catch(Exception)
        {
            repository.RollbackTransaction();
            throw;
        }
    }
}
