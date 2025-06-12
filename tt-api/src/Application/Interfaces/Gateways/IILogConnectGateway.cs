namespace Application.Interfaces.Gateways;

public interface IILogConnectGateway
{
    void PalletTransferRequestCreated(string jobNo);
    void IntegrationStatusChanged();
    void AdjustmentCompleted(string jobNo);
    Task<bool> IsPickingRequestStarted(string pickingRequestId, CancellationToken cancellationToken);
    void PickingRequestCreated(string pickingRequestId, int revision);
}
