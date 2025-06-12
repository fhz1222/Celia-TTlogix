using Application.Exceptions;
using Application.Extensions;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.Customer.Commands.UpdateCustomerClientCommand;

public class UpdateCustomerClientCommand : IRequest<CustomerClient>
{
    public CustomerClientDto CustomerClient { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class UpdateCustomerClientCommandHandler : IRequestHandler<UpdateCustomerClientCommand, CustomerClient>
{
    private readonly IRepository repository;

    public UpdateCustomerClientCommandHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<CustomerClient> Handle(UpdateCustomerClientCommand request, CancellationToken cancellationToken)
    {
        if(!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new UnknownUserCodeException();

        if(!repository.Utils.CheckIfCustomerCodeExists(request.CustomerClient.CustomerCode))
            throw new UnknownCustomerCodeException();

        if(request.CustomerClient.Code.IsEmpty() || request.CustomerClient.Code.Length > 10)
            throw new ApplicationError("Customer client code cannot be empty and can be max 10 characters.");

        if(request.CustomerClient.Name.IsEmpty())
            throw new ApplicationError("Customer client name cannot be empty.");

        if(request.CustomerClient.CompanyCode.IsEmpty())
            throw new ApplicationError("Customer client company code cannot be empty.");

        if(request.CustomerClient.PrimaryAddress.IsEmpty())
            throw new ApplicationError("Customer client address cannot be empty.");

        if(request.CustomerClient.ShippingAddress.IsEmpty())
            throw new ApplicationError("Customer client shipping address cannot be empty.");

        if(request.CustomerClient.BillingAddress.IsEmpty())
            throw new ApplicationError("Customer client billing address cannot be empty.");

        if(request.CustomerClient.Pic1.IsEmpty())
            throw new ApplicationError("Customer client person in charge PIC1 cannot be empty.");

        repository.BeginTransaction();
        try
        {
            if(repository.CompanyProfiles.CompanyProfileDoesNotExistsOrIsInactive(request.CustomerClient.CompanyCode))
                throw new ApplicationError($"Invalid or inactive company profile {request.CustomerClient.CompanyCode}.");

            if(repository.Customers.AddressBookDoesNotExistsOrIsInactive(request.CustomerClient.CompanyCode, request.CustomerClient.PrimaryAddress))
                throw new ApplicationError($"Invalid or inactive address contact {request.CustomerClient.PrimaryAddress}.");

            if(repository.Customers.AddressBookDoesNotExistsOrIsInactive(request.CustomerClient.CompanyCode, request.CustomerClient.ShippingAddress))
                throw new ApplicationError($"Invalid or inactive address contact {request.CustomerClient.ShippingAddress}.");

            if(repository.Customers.AddressBookDoesNotExistsOrIsInactive(request.CustomerClient.CompanyCode, request.CustomerClient.BillingAddress))
                throw new ApplicationError($"Invalid or inactive address contact {request.CustomerClient.BillingAddress}.");

            if(repository.Customers.PICDoesNotExistsOrIsInactive(request.CustomerClient.PrimaryAddress, request.CustomerClient.Pic1))
                throw new ApplicationError($"Invalid or inactive person in charge {request.CustomerClient.Pic1}.");

            if(request.CustomerClient.Pic2.IsNotEmpty() && repository.Customers.PICDoesNotExistsOrIsInactive(request.CustomerClient.PrimaryAddress, request.CustomerClient.Pic2))
                throw new ApplicationError($"Invalid or inactive person in charge {request.CustomerClient.Pic2}.");

            var obj = repository.Customers.TryGetCustomerClient(request.CustomerClient.Code);

            if (obj is null)
            {
                obj = new CustomerClient
                {
                    Code = request.CustomerClient.Code,
                    Name = request.CustomerClient.Name,
                    CustomerCode = request.CustomerClient.CustomerCode,
                    CompanyCode = request.CustomerClient.CompanyCode,
                    PrimaryAddress = request.CustomerClient.PrimaryAddress,
                    BillingAddress = request.CustomerClient.BillingAddress,
                    ShippingAddress = request.CustomerClient.ShippingAddress,
                    Pic1 = request.CustomerClient.Pic1,
                    Pic2 = request.CustomerClient.Pic2,
                    CreatedBy = request.UserCode,
                    CreatedDate = DateTime.Now,
                    CancelledBy = "",
                    CancelledDate = null,
                    RevisedBy = "",
                    RevisedDate = null,
                    Status = CustomerClientStatus.Active,
                };

                repository.Customers.AddNewCustomerClient(obj);
            }
            else
            {
                if (obj.CustomerCode != request.CustomerClient.CustomerCode)
                    throw new ApplicationError($"Customer code modifying is not allowed.");

                obj.Code = request.CustomerClient.Code;
                obj.Name = request.CustomerClient.Name;
                obj.CompanyCode = request.CustomerClient.CompanyCode;
                obj.PrimaryAddress = request.CustomerClient.PrimaryAddress;
                obj.BillingAddress = request.CustomerClient.BillingAddress;
                obj.ShippingAddress = request.CustomerClient.ShippingAddress;
                obj.Pic1 = request.CustomerClient.Pic1;
                obj.Pic2 = request.CustomerClient.Pic2;
                obj.RevisedBy = request.UserCode;
                obj.RevisedDate = DateTime.Now;

                repository.Customers.UpdateCustomerClient(obj);
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
