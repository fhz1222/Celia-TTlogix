using Application.Interfaces.Utils;
using Application.UseCases.CompanyProfiles;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface ICompanyProfileRepository
    {
        void AddNewAddressBook(AddressBook obj);
        void AddNewAddressContact(AddressContact obj);
        bool CompanyProfileDoesNotExistsOrIsInactive(string companyCode);
        AddressBook? GetAddressBook(string code);
        AddressTreeDto GetAddressTreeDto();
        void UpdateAddressBook(AddressBook obj);
    }
}