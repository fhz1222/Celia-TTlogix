using Application.UseCases.Customer;
using Application.UseCases.Customer.Queries.GetCustomerClientList;
using Application.UseCases.Customer.Queries.GetCustomerList;
using Application.UseCases.Customer.Queries.GetUomDecimalList;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        List<CustomerListItemDto> GetCustomerList(GetCustomerListDtoFilter filter, string? orderBy, bool orderByDescending);
        IEnumerable<CustomerDto> GetCustomers();
        Customer? TryGetCustomer(string code, string whsCode);
        void AddNew(Customer customer);
        void Update(Customer customer);
        bool AddressBookDoesNotExistsOrIsInactive(string companyCode, string primaryAddress);
        bool PICDoesNotExistsOrIsInactive(string primaryAddress, string pic);
        T GetInventoryControl<T>(string customerCode) where T : class;
        void AddNewInventoryControl(InventoryControl obj);
        void UpdateInventoryControl(InventoryControl updated);
        List<UomDecimalListItemDto> GetUomDecimalList(GetUomDecimalListDtoFilter filter, string? orderBy, bool orderByDescending);
        UomDecimal? TryGetUomDecimal(string customerCode, string uom);
        void AddNewUomDecimal(UomDecimal obj);
        void UpdateUomDecimal(UomDecimal updated);
        bool UomExists(string uom);
        List<CustomerClientListItemDto> GetCustomerClientList(GetCustomerClientListDtoFilter filter, string? orderBy, bool orderByDescending);
        CustomerClient GetCustomerClient(string code);
        void AddNewCustomerClient(CustomerClient obj);
        void UpdateCustomerClient(CustomerClient updated);
        CustomerClient? TryGetCustomerClient(string code);
        T? TryGetInventoryControl<T>(string customerCode) where T : class;
    }
}