namespace Application.Interfaces.Repositories;

public interface IILogIntegrationRepository
{
    void Disable();
    void Enable();
    bool GetStatus();
    string[] GetWarehouses();
}
