using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using Domain.ValueObjects;
using Persistence.Entities;

namespace Persistence.Repositories;
public class TransactionRepository: ITransactionRepository
    {
    private readonly AppDbContext context;
    private readonly IMapper mapper;
    public TransactionRepository(AppDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public void AddTransaction(InventoryTransaction transaction)
    {
        var entity = mapper.Map<TtInvTransaction>(transaction);
        entity.SystemDateTime = DateTime.Now;
        entity.SystemDate = entity.SystemDateTime.Value.Date;
        context.TtInvTransactions.Add(entity);
    }

    public void AddTransaction(InventoryTransactionPerSupplier transaction)
    {
        var entity = mapper.Map<TtInvTransactionPerSupplier>(transaction);
        entity.SystemDateTime = DateTime.Now;
        entity.SystemDate = entity.SystemDateTime.Date;
        context.TtInvTransactionPerSuppliers.Add(entity);
    }

    public void AddTransaction(InventoryTransactionPerWhsCode transaction)
    {
        var entity = mapper.Map<TtInvTransactionPerWh>(transaction);
        entity.SystemDateTime = DateTime.Now;
        entity.SystemDate = entity.SystemDateTime.Value.Date;
        context.TtInvTransactionPerWhs.Add(entity);
    }

    public InventoryTransaction? GetLatestTransaction(string customerCode, string productCode)
    {
        var transaction = context.TtInvTransactions
            .Where(t => t.CustomerCode == customerCode && t.ProductCode == productCode)
            .OrderByDescending(t => t.SystemDateTime)
            .FirstOrDefault();
        if (transaction == null)
            return null;
        return mapper.Map<InventoryTransaction>(transaction);
    }

    public InventoryTransactionPerSupplier? GetLatestTransactionPerSupplier(string customerCode, string productCode, string supplierId, Ownership ownership)
    {
        var transaction = context.TtInvTransactionPerSuppliers
            .Where(t => t.CustomerCode == customerCode && t.ProductCode == productCode && t.SupplierId == supplierId && t.Ownership == (byte) ownership)
            .OrderByDescending(t => t.SystemDateTime)
            .FirstOrDefault();
        if (transaction == null)
            return null;
        return mapper.Map<InventoryTransactionPerSupplier>(transaction);
    }

    public InventoryTransactionPerWhsCode? GetLatestTransactionPerWhsCode(string customerCode, string productCode, string whsCode)
    {
        var transaction = context.TtInvTransactionPerWhs
            .Where(t => t.CustomerCode == customerCode && t.ProductCode == productCode && t.Whscode == whsCode)
            .OrderByDescending(t => t.SystemDateTime)
            .FirstOrDefault();
        if (transaction == null)
            return null;
        return mapper.Map<InventoryTransactionPerWhsCode>(transaction);
    }

    public Pallet? GetPalletDetail(string palletId)
    {
        var storageDetail = context.TtStorageDetails.Find(palletId);
        if (storageDetail == null)
            return null;
        var result = mapper.Map<Pallet>(storageDetail);
        // map product details
        var partMaster = context.TtPartMasters.Find(storageDetail.CustomerCode, storageDetail.SupplierId, storageDetail.ProductCode);
        result.Product = mapper.Map<Product>(partMaster);
        return result;
    }
}
