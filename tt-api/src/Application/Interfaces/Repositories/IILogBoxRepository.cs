using Application.UseCases.ILogIntegration;

namespace Application.Interfaces.Repositories;

public interface IILogBoxRepository
{
    void DeletePalletBoxes(string palletId);
    void CreateBox(string palletId, int qty);
    List<BoxDto> GetBoxes(string palletId);
    List<BoxDto> GetBoxes(string[] palletIds);
    BoxDto? GetBox(string boxId);
    void DeleteBoxes(string[] boxIds);
    void DeleteAllBoxes();
    void UpdateBoxes(BoxDto[] boxes);
    bool AreBoxesGenerated();
}
