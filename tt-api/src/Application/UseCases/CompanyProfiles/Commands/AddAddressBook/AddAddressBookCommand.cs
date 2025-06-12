using Application.Exceptions;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Application.UseCases.CompanyProfiles.Commands.AddAddressBook;

public class AddAddressBookCommand : IRequest<AddressBook>
{
    public AddAddressBookDto AddressBook { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class UpdateAddressBookCommandHandler : IRequestHandler<AddAddressBookCommand, AddressBook>
{
    private readonly IRepository repository;
    private readonly IValidator<AddAddressBookDto>? validator;

    public UpdateAddressBookCommandHandler(IRepository repository, IValidator<AddAddressBookDto>? validator)
    {
        this.repository = repository;
        this.validator = validator;
    }

    public async Task<AddressBook> Handle(AddAddressBookCommand request, CancellationToken cancellationToken)
    {
        if(!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new UnknownUserCodeException();

        if(validator is not null && validator.Validate(request.AddressBook) is var validationResult && !validationResult.IsValid)
        {
            var errorMessage = string.Join(' ', validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ApplicationError(errorMessage);
        }

        repository.BeginTransaction();
        try
        {
            var code = request.AddressBook.CompanyCode
                + repository.Utils.GetAddressBookAutoNum(request.AddressBook.CompanyCode);
            var obj = new AddressBook
            {
                Code = code,
                CompanyCode = request.AddressBook.CompanyCode,
                Address1 = request.AddressBook.Address1,
                Address2 = request.AddressBook.Address2,
                Address3 = request.AddressBook.Address3,
                Address4 = request.AddressBook.Address4,
                PostCode = request.AddressBook.PostCode,
                Country = request.AddressBook.Country,
                TelNo = request.AddressBook.TelNo,
                FaxNo = request.AddressBook.FaxNo,
                CreateBy = request.UserCode,
                CreateDate = DateTime.Now,
                CancelledBy = "",
                CancelledDate = null,
                RevisedBy = "",
                RevisedDate = null,
                Status = Status.Active,
            };

            repository.CompanyProfiles.AddNewAddressBook(obj);

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
