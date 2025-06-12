using Application.UseCases.Registration.Commands.AddArea;
using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Repositories;

public interface IUtilsRepository
{
    string GetJobCode(CodePrefix codePrefix);
    bool CheckIfWhsCodeExists(string code);
    bool CheckIfCustomerCodeExists(string code);
    bool CheckIfUserCodeExists(string code);
    bool CheckIfAreaTypeExists(string code);
    bool CheckIfAreaExists(string code, string whsCode);
    Owner? GetOwner(string code);
    Task<AccessLock?> GetAccessLockAsync(string jobNo);
    string GetAddressBookAutoNum(string companyCode);
    string GetNextCode(CodePrefix codePrefix);
}
