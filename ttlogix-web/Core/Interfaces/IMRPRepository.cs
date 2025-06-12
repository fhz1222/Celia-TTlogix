using System.Threading.Tasks;
using TT.Core.Entities.MRP;

namespace TT.Core.Interfaces
{
    public interface IMRPRepository
    {
        Task<MRPItemMaster> GetItemMasterAsync(string factoryID, string supplierID, string productCode);
        Task<MRPInventory> GetInventoryAsync(string factoryID, string supplierID, string productCode);
        Task AddItemMasterAsync(MRPItemMaster entity);
        Task AddInventoryAsync(MRPInventory entity);
        Task SaveChangesAsync();
    }
}
