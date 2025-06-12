using Application.UseCases.ILogLocationIntegration;
using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Repositories;

public interface ILocationRepository
{
    int GetILogStorageLocationCategoryId();
    int GetILogInboundLocationCategoryId();
    Location? GetLocation(string location, string wHSCode);
    bool IsILogInboundLocation(string locationCode, string whsCode);
    bool IsILogStorageLocation(string locationCode, string whsCode);
    void ResetLocationCategories();
    void RestoreILogSystemLocations(string whsCode);
    bool IsLocationOfCategory(string locationCode, string whsCode, string categoryNames);
    IEnumerable<ILogIntegrationLocationDto> GetLocationsForWHS(string[] WHSCodes);
    void AddLocations(IEnumerable<ILogIntegrationLocationDto> items);
    void ActivateLocations(IEnumerable<ILogIntegrationLocationIdDto> items);
    void DeactivateLocations(IEnumerable<ILogIntegrationLocationIdDto> items);
    void UpdateLocations(IEnumerable<ILogIntegrationLocationDto> items);
    string GetILogLocationCategoryName(string locationCode, string whsCode);
}
