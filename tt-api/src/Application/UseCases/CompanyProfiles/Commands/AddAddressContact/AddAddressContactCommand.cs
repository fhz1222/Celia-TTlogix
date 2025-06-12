using Application.Exceptions;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Application.UseCases.CompanyProfiles.Commands.AddAddressContact;

public class AddAddressContactCommand : IRequest<AddressContact>
{
    public AddAddressContactDto AddressContact { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class UpdateAddressContactCommandHandler : IRequestHandler<AddAddressContactCommand, AddressContact>
{
    private readonly IRepository repository;
    private readonly IValidator<AddAddressContactDto>? validator;

    public UpdateAddressContactCommandHandler(IRepository repository, IValidator<AddAddressContactDto>? validator)
    {
        this.repository = repository;
        this.validator = validator;
    }

    public async Task<AddressContact> Handle(AddAddressContactCommand request, CancellationToken cancellationToken)
    {
        if(!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new UnknownUserCodeException();

        if(validator is not null && validator.Validate(request.AddressContact) is var validationResult && !validationResult.IsValid)
        {
            var errorMessage = string.Join(' ', validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ApplicationError(errorMessage);
        }

        repository.BeginTransaction();
        try
        {
            var obj = new AddressContact
            {
                Code = repository.Utils.GetNextCode(Domain.Enums.CodePrefix.AddressContact),
                AddressCode = request.AddressContact.AddressCode,
                Name = request.AddressContact.Name,
                Email = request.AddressContact.Email ?? "",
                TelNo = request.AddressContact.TelNo,
                FaxNo = request.AddressContact.FaxNo,
                CreatedBy = request.UserCode,
                CreatedDate = DateTime.Now,
                CancelledBy = "",
                CancelledDate = null,
                RevisedBy = "",
                RevisedDate = null,
                Status = Status.Active,
            };

            repository.CompanyProfiles.AddNewAddressContact(obj);

            await repository.SaveChangesAsync(cancellationToken);
            repository.CommitTransaction();
            return obj;
        }
        catch(Exception)
        {
            repository.RollbackTransaction();
            throw;
        }
    }
}
