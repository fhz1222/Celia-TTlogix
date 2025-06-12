using System.Threading.Tasks;
using TT.Core.Entities.MRP;
using TT.Core.Interfaces;

namespace TT.DB
{
    public class SqlMRPRepository : IMRPRepository
    {
        public SqlMRPRepository(MRPContext dbContext) => this.dbContext = dbContext;

        public async Task AddInventoryAsync(MRPInventory entity)
        {
            dbContext.MRPInventory.Add(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task AddItemMasterAsync(MRPItemMaster entity)
        {
            dbContext.MRPItemMasters.Add(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task<MRPInventory> GetInventoryAsync(string factoryID, string supplierID, string productCode)
            => await dbContext.MRPInventory.FindAsync(factoryID, supplierID, productCode);

        public async Task<MRPItemMaster> GetItemMasterAsync(string factoryID, string supplierID, string productCode)
            => await dbContext.MRPItemMasters.FindAsync(factoryID, supplierID, productCode);

        public async Task SaveChangesAsync()
            => await dbContext.SaveChangesAsync();

        private readonly MRPContext dbContext;
    }
}
