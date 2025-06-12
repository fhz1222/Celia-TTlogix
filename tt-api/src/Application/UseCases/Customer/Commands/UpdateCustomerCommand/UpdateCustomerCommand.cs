using Application.Exceptions;
using Application.Extensions;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Customer.Commands.UpdateCustomerCommand;

public class UpdateCustomerCommand : IRequest<Domain.Entities.Customer>
{
    public UpdateCustomerDto Customer { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Domain.Entities.Customer>
{
    private readonly IRepository repository;

    public UpdateCustomerCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Domain.Entities.Customer> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        if(!repository.Utils.CheckIfWhsCodeExists(request.Customer.WhsCode))
            throw new UnknownWhsCodeException();

        if(!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new UnknownUserCodeException();

        if(request.Customer.Code.IsEmpty() || request.Customer.Code.Length > 10)
            throw new ApplicationError("Customer code cannot be empty and can be max 10 characters.");

        if(request.Customer.Name.IsEmpty())
            throw new ApplicationError("Customer name cannot be empty.");

        if(request.Customer.CompanyCode.IsEmpty())
            throw new ApplicationError("Customer company code cannot be empty.");

        if(request.Customer.PrimaryAddress.IsEmpty())
            throw new ApplicationError("Customer address cannot be empty.");

        if(request.Customer.ShippingAddress.IsEmpty())
            throw new ApplicationError("Customer shipping address cannot be empty.");

        if(request.Customer.BillingAddress.IsEmpty())
            throw new ApplicationError("Customer billing address cannot be empty.");

        if(request.Customer.Pic1.IsEmpty())
            throw new ApplicationError("Customer person in charge PIC1 cannot be empty.");

        if(request.Customer.BizUnit.Length > 10)
            throw new ApplicationError("Customer BizUnit can be max 10 characters.");

        repository.BeginTransaction();
        try
        {
            if(repository.CompanyProfiles.CompanyProfileDoesNotExistsOrIsInactive(request.Customer.CompanyCode))
                throw new ApplicationError($"Invalid or inactive company profile {request.Customer.CompanyCode}.");

            if(repository.Customers.AddressBookDoesNotExistsOrIsInactive(request.Customer.CompanyCode, request.Customer.PrimaryAddress))
                throw new ApplicationError($"Invalid or inactive address contact {request.Customer.PrimaryAddress}.");

            if(repository.Customers.AddressBookDoesNotExistsOrIsInactive(request.Customer.CompanyCode, request.Customer.ShippingAddress))
                throw new ApplicationError($"Invalid or inactive address contact {request.Customer.ShippingAddress}.");

            if(repository.Customers.AddressBookDoesNotExistsOrIsInactive(request.Customer.CompanyCode, request.Customer.BillingAddress))
                throw new ApplicationError($"Invalid or inactive address contact {request.Customer.BillingAddress}.");

            if(repository.Customers.PICDoesNotExistsOrIsInactive(request.Customer.PrimaryAddress, request.Customer.Pic1))
                throw new ApplicationError($"Invalid or inactive person in charge {request.Customer.Pic1}.");

            if(request.Customer.Pic2.IsNotEmpty() && repository.Customers.PICDoesNotExistsOrIsInactive(request.Customer.PrimaryAddress, request.Customer.Pic2))
                throw new ApplicationError($"Invalid or inactive person in charge {request.Customer.Pic2}.");

            var obj = repository.Customers.TryGetCustomer(request.Customer.Code, request.Customer.WhsCode);

            if (obj is null)
            {
                obj = new Domain.Entities.Customer
                {
                    Code = request.Customer.Code,
                    Whscode = request.Customer.WhsCode,
                    Name = request.Customer.Name,
                    CompanyCode = request.Customer.CompanyCode,
                    BizUnit = request.Customer.BizUnit,
                    Buname = request.Customer.Buname,
                    PrimaryAddress = request.Customer.PrimaryAddress,
                    BillingAddress = request.Customer.BillingAddress,
                    ShippingAddress = request.Customer.ShippingAddress,
                    Pic1 = request.Customer.Pic1,
                    Pic2 = request.Customer.Pic2,
                    CreatedBy = request.UserCode,
                    CreatedDate = DateTime.Now,
                    CancelledBy = "",
                    CancelledDate = null,
                    RevisedBy = "",
                    RevisedDate = null,
                    Status = Domain.Enums.CustomerStatus.Active,
                };

                repository.Customers.AddNew(obj);
            }
            else
            {
                obj.Name = request.Customer.Name;
                obj.CompanyCode = request.Customer.CompanyCode;
                obj.BizUnit = request.Customer.BizUnit;
                obj.Buname = request.Customer.Buname;
                obj.PrimaryAddress = request.Customer.PrimaryAddress;
                obj.BillingAddress = request.Customer.BillingAddress;
                obj.ShippingAddress = request.Customer.ShippingAddress;
                obj.Pic1 = request.Customer.Pic1;
                obj.Pic2 = request.Customer.Pic2;
                obj.RevisedBy = request.UserCode;
                obj.RevisedDate = DateTime.Now;

                repository.Customers.Update(obj);
            }

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
