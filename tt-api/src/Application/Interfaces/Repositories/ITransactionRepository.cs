using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Interfaces.Repositories;

public interface ITransactionRepository
{
    InventoryTransactionPerSupplier? GetLatestTransactionPerSupplier(string customerCode, string productCode, string supplierId, Ownership ownership);
    InventoryTransaction? GetLatestTransaction(string customerCode, string productCode);
    InventoryTransactionPerWhsCode? GetLatestTransactionPerWhsCode(string customerCode, string productCode, string whsCode);
    void AddTransaction(InventoryTransaction transaction);
    void AddTransaction(InventoryTransactionPerSupplier transaction);
    void AddTransaction(InventoryTransactionPerWhsCode transaction);
}
