namespace Application.Interfaces.Repositories;

public interface IStockTransferRepository
{
    IEnumerable<string> GetStockTransferPalletsOnLocationCategory(string jobNo, int locationCategoryId);
}
